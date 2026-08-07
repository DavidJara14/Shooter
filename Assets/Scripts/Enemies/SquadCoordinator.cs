using UnityEngine;

public static class SquadCoordinator
{
    private const float RadioDeAlerta = 25f;

    public static void AlertarCercanos(AIAgentBase quienDetecto, Transform objetivo)
    {
        Collider[] cercanos = Physics.OverlapSphere(quienDetecto.transform.position, RadioDeAlerta);

        foreach (Collider col in cercanos)
        {
            AIAgentBase otroAgente = col.GetComponentInParent<AIAgentBase>();
            if (otroAgente == null || otroAgente == quienDetecto)
            {
                continue;
            }
            if (otroAgente.FactionActual != quienDetecto.FactionActual) // para no notificar a enemigos
            {
                continue; 
            }

            otroAgente.OnObjetivoDetectado(objetivo);

        }
    }
}
