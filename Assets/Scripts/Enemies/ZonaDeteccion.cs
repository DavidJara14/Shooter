using UnityEngine;

public class ZonaDeteccion : MonoBehaviour
{
    public enum TipoZona { Deteccion, Ataque }

    [SerializeField] private TipoZona tipo = TipoZona.Deteccion;
    [SerializeField] private string tagObjetivo = "Player";

    public AIAgentBase agente; 

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagObjetivo))
        {
            return;
        }
        if (tipo == TipoZona.Deteccion)
        {
            agente.OnObjetivoDetectado(other.transform);
        }
        if(tipo == TipoZona.Ataque)
        {
            agente.OnEntroRangoDeAtaque();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagObjetivo))
        {
            return;
        }
        if (tipo == TipoZona.Deteccion)
        {
            agente.OnObjetivoPerdido();
        }
        if (tipo == TipoZona.Ataque)
        {
            agente.OnSalioRangoDeAtaque();
        }
    }

}
