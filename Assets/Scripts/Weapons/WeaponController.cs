using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask hittableLayers = -0; //todas las layers del juego 
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private int currentAmmo;
    private float nextFireTime;

    private void Start()
    {
        currentAmmo = weaponData.magazineSize;
    }

    public void TryFire()
    {
        if (Time.time < nextFireTime) // todavia no puede volver a disparar (tiempo de espera)
        {
            return;
        }

        if (currentAmmo<=0) // sin municion
        {
            return; 
        }

        // si puede disparar
        nextFireTime = Time.time + (1f/weaponData.fireRate);
        currentAmmo --; // cada disparo es una bala menos

        DispararRayo();
    }

    private void DispararRayo()
    {
        RaycastHit hit;
        bool golpeo = Physics.Raycast(transform.position, transform.forward, out hit, weaponData.range, hittableLayers);
        Vector3 puntoFinal;

        if (golpeo)
        {
            puntoFinal = hit.point;

            IDamageable damageable = hit.collider.GetComponent<IDamageable>(); // intentando obtener el componente IDamageable del objeto al que disparamos

            if (damageable != null)
            {
                damageable.TakeDamage(weaponData.damage, hit.point, hit.normal, gameObject); // haciendo daño al objeto/enemigo
            }

            Debug.Log("El disparo golpeo a: " + hit.collider.name);
        }
        else
        {
            puntoFinal = firePoint.position + transform.forward * weaponData.range;
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
}
