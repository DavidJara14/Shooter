using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InstanciarYDestruir : MonoBehaviour
{
    [SerializeField] private float alcance = 50f;
    [SerializeField] private GameObject prefabParaInstanciar;

    void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            DispararYCrear();
        }
    }

    private void DispararYCrear()
    {
        RaycastHit hit;
        bool golpeo = Physics.Raycast(transform.position, transform.forward, out hit, alcance);
        Vector3 puntoFinal;

        if (golpeo)
        {
            puntoFinal = hit.point;
            Debug.Log("El rayo golpeo a: " + hit.collider.name);
            GameObject nuevoObjeto = Instantiate(prefabParaInstanciar, hit.point, Quaternion.identity);

            Destroy(nuevoObjeto, 5f);
            Debug.Log("Se destruyo el objeto con el nombre: " + nuevoObjeto.name);
        }
        else
        {
            puntoFinal = transform.position + transform.forward * alcance;
            Debug.Log("El rayo no golpeo nada");
        }

    }
}
