using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputShoot : MonoBehaviour
{
    public WeaponController weaponController;

    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            weaponController.TryFire();
        }
    }
}
