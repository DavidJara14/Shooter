using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    public Animator animator;
    public float tiempoAntesDeDestruir = 4f;
    public Health health;

    private void OnEnable()
    {
        health.OnDeath += ManejarMuerte;
    }
    private void OnDisable()
    {
        health.OnDeath -= ManejarMuerte;
    }


    private void ManejarMuerte(GameObject atacante)
    {
        animator?.SetTrigger("Die");
        Destroy(gameObject, tiempoAntesDeDestruir);
    }


}
