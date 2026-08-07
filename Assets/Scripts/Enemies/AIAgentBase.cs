using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public abstract class AIAgentBase : MonoBehaviour
{
    protected enum Estado { Patrulla, Persecucion, Ataque }

    [Header("Identidad")]
    [SerializeField] protected Faction faction = Faction.Enemy;
    public Faction FactionActual => faction;

    [Header("Patrulla")]
    [SerializeField] protected Transform[] puntosDePatrulla;
    [SerializeField] protected float velocidadPatrulla = 2f;
    [SerializeField] protected float esperaEnPunto = 2f;

    [Header("Persecución")]
    [SerializeField] protected float velocidadPersecucion = 4.5f;
    [SerializeField] protected float intervaloRecalculoRuta = 0.2f;

    [Header("Referencias")]
    [SerializeField] protected Animator animator;

    protected NavMeshAgent agent;
    protected Health health;
    protected Estado estadoActual = Estado.Patrulla;
    protected Transform objetivoActual;

    private int indicePatrulla;
    private float temporizadorEspera;
    private float siguienteRecalculo;

    private bool alertaPendiente;
    private Transform objetivoParaAlerta;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
    }

    protected virtual void OnEnable() { health.OnDeath += ManejarMuerte; }
    protected virtual void OnDisable() { health.OnDeath -= ManejarMuerte; }

    protected virtual void Update()
    {
        if (health.isDead) return;

        if (alertaPendiente)
        {
            alertaPendiente = false;
            SquadCoordinator.AlertarCercanos(this, objetivoParaAlerta);
        }

        switch (estadoActual)
        {
            case Estado.Patrulla: ActualizarPatrulla(); break;
            case Estado.Persecucion: ActualizarPersecucion(); break;
            case Estado.Ataque: ActualizarAtaque(); break;
        }

        ActualizarAnimator();
    }

    protected virtual void ActualizarPatrulla()
    {
        agent.speed = velocidadPatrulla;
        if (puntosDePatrulla == null || puntosDePatrulla.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            temporizadorEspera += Time.deltaTime;
            if (temporizadorEspera >= esperaEnPunto)
            {
                indicePatrulla = (indicePatrulla + 1) % puntosDePatrulla.Length;
                agent.SetDestination(puntosDePatrulla[indicePatrulla].position);
                temporizadorEspera = 0f;
            }
        }
    }

    protected virtual void ActualizarPersecucion()
    {
        if (objetivoActual == null) { CambiarEstado(Estado.Patrulla); return; }

        agent.speed = velocidadPersecucion;

        if (Time.time >= siguienteRecalculo)
        {
            agent.SetDestination(objetivoActual.position);
            siguienteRecalculo = Time.time + intervaloRecalculoRuta;
        }
    }

    protected virtual void ActualizarAtaque()
    {
        if (objetivoActual == null) { CambiarEstado(Estado.Patrulla); return; }
        MirarHaciaObjetivo();
        EjecutarAtaque(); // se dispara cada Update normal, no desde ningún trigger
    }

    protected void MirarHaciaObjetivo()
    {
        Vector3 direccion = objetivoActual.position - transform.position;
        direccion.y = 0f;
        if (direccion.sqrMagnitude < 0.01f) return;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 8f);
    }

    protected virtual void ActualizarAnimator()
    {
        if (animator == null) return;

        float velocidadNormalizada = agent.velocity.magnitude / Mathf.Max(velocidadPersecucion, 0.01f);
        bool moviendose = velocidadNormalizada > 0.05f;
        bool corriendo = agent.speed >= velocidadPersecucion - 0.01f;

        animator.SetBool("IsWalking", moviendose);
        animator.SetBool("IsRunning", moviendose && corriendo);
    }

    public virtual void OnObjetivoDetectado(Transform objetivo)
    {
        if (health.isDead) return;

        bool esDeteccionNueva = (objetivoActual != objetivo) || (estadoActual == Estado.Patrulla);

        objetivoActual = objetivo;
        if (estadoActual == Estado.Patrulla) CambiarEstado(Estado.Persecucion);

        if (esDeteccionNueva)
        {
            alertaPendiente = true;
            objetivoParaAlerta = objetivo;
        }
    }

    protected virtual void CambiarEstado(Estado nuevoEstado)
    {
        estadoActual = nuevoEstado;
        agent.isStopped = (nuevoEstado == Estado.Ataque);
        agent.updateRotation = (nuevoEstado != Estado.Ataque);
    }

    public virtual void OnObjetivoPerdido()
    {
        CancelInvoke(nameof(ConfirmarPerdidaDeObjetivo));
        Invoke(nameof(ConfirmarPerdidaDeObjetivo), 2f);
    }

    private void ConfirmarPerdidaDeObjetivo()
    {
        if (estadoActual == Estado.Ataque) return;
        objetivoActual = null;
        CambiarEstado(Estado.Patrulla);
    }

    public virtual void OnEntroRangoDeAtaque()
    {
        if (health.isDead || objetivoActual == null) return;
        CambiarEstado(Estado.Ataque);
    }

    public virtual void OnSalioRangoDeAtaque()
    {
        if (objetivoActual != null) CambiarEstado(Estado.Persecucion);
    }

    protected abstract void EjecutarAtaque();

    private void ManejarMuerte(GameObject atacante)
    {
        agent.isStopped = true;
        enabled = false;
    }
}