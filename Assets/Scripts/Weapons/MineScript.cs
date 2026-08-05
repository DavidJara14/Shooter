using System;
using UnityEngine;

public class MineScript : MonoBehaviour
{
    [SerializeField] private SphereCollider zonaDeteccion;
    [SerializeField] private string tagActiva = "Player";

    public float daño = 100f;
    public float radioExplosion = 4f;
    public GameObject explosionPrefab;
    public AudioClip sonidoExplosion;
    public LayerMask juegadoresAfectados;

    public bool yaExploto = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider tag" + other.tag);
        if (other.CompareTag(tagActiva))
        {

            Explotar();
        }
    }

    private void Explotar()
    {
        if (yaExploto)
        {
            return;
        }
        yaExploto = true;

        Collider[] afectados = Physics.OverlapSphere(transform.position, radioExplosion, juegadoresAfectados); // detectando los jugadores dentro del radio de explosion

        foreach (Collider col in afectados)
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            damageable?.TakeDamage(daño, transform.position, Vector3.up, gameObject);
        }

        GameObject explotionInstantiate = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(sonidoExplosion, transform.position);
        Destroy(explotionInstantiate, 3f);
        Destroy(gameObject);
    }
}
