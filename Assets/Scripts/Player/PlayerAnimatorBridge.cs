using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimatorBridge : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float velocidadSprintAnim = 1.6f;

    private PlayerMovement playerMovement;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        playerMovement.OnSalto += ManejarSalto;
    }

    void OnDisable()
    {
        playerMovement.OnSalto -= ManejarSalto;
    }

    void Update()
    {
        animator.SetFloat("VelX", playerMovement.MoveX);
        animator.SetFloat("VelZ", playerMovement.MoveY);

        // en vez de clips de "correr" separados, aceleramos el playback
        animator.speed = playerMovement.estaCorriendo ? velocidadSprintAnim : 1f;
    }

    private void ManejarSalto()
    {
        animator.SetTrigger("Jump");
    }
}