using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask hittableLayers = ~0; //todas las layers del juego 
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Efectos")]
    [SerializeField] private ParticleSystem muzzleFlash; //Efecto de disparo
    [SerializeField] private AudioSource audioSource;    //Sonido de disparo
    [SerializeField] private GameObject bloodHitEffectPrefab;

    [Header("Recarga")]
    public Transform magVisual;
    public float tiempoRecarga = 1.8f;
    public float fuerzaCaidaMag;

    public Transform FirePoint => firePoint;
    public bool TieneMunicion => currentAmmo > 0;
    public bool EstaRecargando { get; private set; }


    [SerializeField] private int currentAmmo;
    private float nextFireTime;

    private void Start()
    {
        currentAmmo = weaponData.magazineSize;
    }

    public bool TryFire(Vector3? direccionOverride = null) // devuelve si se pudo realizar el disparo
    {
        if (EstaRecargando)
        {
            return false;
        }

        if (Time.time < nextFireTime) // todavia no puede volver a disparar (tiempo de espera)
        {
            return false;
        }

        if (currentAmmo<=0) // sin municion
        {
            return false; 
        }

        // si puede disparar
        nextFireTime = Time.time + (1f/weaponData.fireRate);
        currentAmmo --; // cada disparo es una bala menos

        muzzleFlash.Play(); //Renderizar el flash del disparo
        audioSource.PlayOneShot(weaponData.fireSound); // reproduciendo el sonido de disparo una vez

        DispararRayo(direccionOverride);
        return true;
    }

    private void DispararRayo(Vector3? direccionOverride = null)
    {
        Vector3 direccion = direccionOverride ?? firePoint.forward;

        RaycastHit hit;
        bool golpeo = Physics.Raycast(firePoint.position, direccion, out hit, weaponData.range, hittableLayers);
                            //Raycast(punto_de_inicio, vector de dereccion, informacion de salida, alcance del raycast, layer_mask(capas afectadas)) 
        Vector3 puntoFinal;

        if (golpeo)
        {
            puntoFinal = hit.point;

            IDamageable damageable = hit.collider.GetComponent<IDamageable>(); // intentando obtener el componente IDamageable del objeto al que disparamos

            if (damageable != null)
            {
                damageable.TakeDamage(weaponData.damage, hit.point, hit.normal, gameObject); // haciendo daño al objeto/enemigo

                if (bloodHitEffectPrefab != null)
                {
                    Quaternion rotacionImpacto = Quaternion.LookRotation(hit.normal);
                    Instantiate(bloodHitEffectPrefab, hit.point, rotacionImpacto);
                }
            }

            Debug.Log("El disparo golpeo a: " + hit.collider.name);
        }
        else
        {
            puntoFinal = firePoint.position + direccion * weaponData.range;
            Debug.Log("El disparo no golpeo nada");
        }
        MostrarLinea(firePoint.position, puntoFinal);
    }

    private void MostrarLinea(Vector3 origen, Vector3 destino)
    {
        lineRenderer.SetPosition(0, origen);
        lineRenderer.SetPosition(1, destino);
        lineRenderer.enabled = true;

        Invoke(nameof(OcultarLinea), 0.25f); // Invoke - llamar una funcion despues de cierto tiempo
    }

    private void OcultarLinea()
    {
        lineRenderer.enabled = false;
    }


    public void IniciarRecarga()
    {
        if (EstaRecargando || currentAmmo == weaponData.magazineSize)
        {
            return;
        }
        StartCoroutine(RecargarCorutina());
    }

    private IEnumerator RecargarCorutina()
    {
        EstaRecargando = true;
        SoltarMagAlPiso();
        yield return new WaitForSeconds(tiempoRecarga);
        currentAmmo = weaponData.magazineSize;
        if (magVisual != null)
        {
            magVisual.gameObject.SetActive(true);
        }
        EstaRecargando = false;
    }

    private void SoltarMagAlPiso() 
    {
        if(magVisual == null)
        {
            return;
        }

        GameObject magCaida = Instantiate(magVisual.gameObject, magVisual.position, magVisual.rotation);
        magVisual.transform.SetParent(null);
        Rigidbody rb = magCaida.GetComponent<Rigidbody>(); // recordar añadir el componente RigidBody al mag

        Vector3 direccionAleatoria = new Vector3(Random.Range(-0.3f,0.3f), 0.3f, Random.Range(-0.3f, 0.3f));

        rb.AddForce(direccionAleatoria * fuerzaCaidaMag, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * fuerzaCaidaMag, ForceMode.Impulse);

        Destroy(magCaida, 5f);
        magVisual.gameObject.SetActive(false);
    }
}
