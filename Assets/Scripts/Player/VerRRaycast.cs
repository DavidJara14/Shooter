using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class VerRRaycast : MonoBehaviour
{
    [SerializeField] private float alcance = 50f;
    [SerializeField] private LineRenderer lineRenderer;

    void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            DispararRayo();
        }
    }

    private void DispararRayo()
    {
        RaycastHit hit;
        bool golpeo = Physics.Raycast(transform.position, transform.forward, out hit, alcance);
        Vector3 puntoFinal;

        if (golpeo)
        {
            puntoFinal = hit.point;
            Debug.Log("El rayo golpeo a: " + hit.collider.name);
        }
        else
        {
            puntoFinal = transform.position + transform.forward * alcance;
            Debug.Log("El rayo no golpeo nada");
        }
        MostrarLinea(transform.position, puntoFinal);
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
