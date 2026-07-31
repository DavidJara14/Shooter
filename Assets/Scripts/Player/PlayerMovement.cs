using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidad")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;

    [Header("Salto y gravedad")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.8f;

    [SerializeField] private CharacterController controller; 
    private Vector2 moveInput; // almacenar las lecturas de teclado 
    // las variables de tipo bool tienen 2 estados (true/false) o (verdadero y falso)
    private bool jumpRequest; 
    private bool sprintHeld;

    private float velocity;

    public event Action OnSalto;
    public float MoveX => moveInput.x;
    public float MoveY => moveInput.y;
    public bool estaCorriendo => sprintHeld;


    private void Awake() // Se ejecuta cuando se inicializa el juego (antes de la funcion start)
    {
        //controller = GetComponent<CharacterController>(); // obteniendo el componente Character Controller del Player
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        ApplyMovement();
    }

    private void ReadInput()
    {
        Keyboard keyboard = Keyboard.current;
        float x = 0f;
        float z = 0f;

        if (keyboard.wKey.isPressed)
        {
            z += 1f; // adelante
        }
        if (keyboard.sKey.isPressed)
        {
            z -= 1f; // atras
        }
        if (keyboard.dKey.isPressed)
        {
            x += 1f; // derecha
        }
        if (keyboard.aKey.isPressed)
        {
            x -= 1f; // izquierda
        }

        moveInput = new Vector2(x, z); // vector de movimiento
        sprintHeld = keyboard.leftShiftKey.isPressed; // detectar si la tecla shift izquierda esta presionada

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpRequest = true;
        }
    }

    private void ApplyMovement()
    {
        Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        float currentSpeed = walkSpeed; // por default la velocidad del player es la walkSpeed (5f)
        if (sprintHeld == true)
        {
            currentSpeed = sprintSpeed; // solo se actualiza el valor de velocidad si se presiona la tecla leftShift
        }

        if (controller.isGrounded) // detectar si el player esta sobre una superficie
        {
            velocity = -2f; // pequeña fuerza para evitar falsos positivos 
            if (jumpRequest)
            {
                velocity = Mathf.Sqrt(-2*jumpHeight*gravity); // calculando la velocidad final de caida en el salto
                                                              // V = raiz(2*altura*gravedad)
                OnSalto?.Invoke();
            }
        }
        else
        {
            velocity += gravity * Time.deltaTime; 
        }
        jumpRequest = false; 

        Vector3 finalMove = moveDirection * currentSpeed;
        finalMove.y = velocity;
        controller.Move(finalMove * Time.deltaTime);
    }
}