using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;

    [Header("Sensibilidad")]
    [SerializeField] private float sensitivityX = 2f;
    [SerializeField] private float sensitivityY = 2f;

    [Header("Límites")]
    [SerializeField] private float maxY = 85f;
    [SerializeField] private float minY = -85f;

    private float currentRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // centrar el cursor

        if(cameraTransform == null) // la camara no ha sido asignada
        {
            Debug.LogError("Aún no se asigno la cámara");
        }

    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current; // creando un objeto para acceder a la informacion del movimiento del mouse

        // estructura condicional, sirve para evaluar si una condicion es verdadera o falsa y realizar una tarea dependiendo el estado
        if (mouse == null || cameraTransform == null) // si no existe el mouse ni la camara, regresa
        {
            return;
        }

        //Almacenando el movimiento del mouse en un arreglo de 2 dimensiones [pos_y,pos_y]
        Vector2 mouseDelta = mouse.delta.ReadValue();

        // calculando el movimiento de rotacion en X y Y
        float mouseX = mouseDelta.x * sensitivityX;
        float mouseY = mouseDelta.y * sensitivityY;

        // Yaw 
        transform.Rotate(Vector3.up, mouseX);

        //Pitch
        currentRotation -= mouseY; //acumulando la posicion de la lectura del mouse en "Y"
        currentRotation = Mathf.Clamp(currentRotation, minY, maxY); // reestringiendo el movimiento de rotacion
        cameraTransform.localRotation = Quaternion.Euler(currentRotation, 0f, 0f);
    }
}
