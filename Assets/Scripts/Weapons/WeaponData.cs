using UnityEngine;

[CreateAssetMenu(fileName = "NuevaArma", menuName = "ShooterMenu/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "";
    public Sprite icon;

    public float damage = 25f;
    public float fireRate = 8f;
    public float range = 100f;

    public int magazineSize = 30;
}
