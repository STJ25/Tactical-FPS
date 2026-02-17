using UnityEngine;

[CreateAssetMenu(fileName = "ShootConfiguration", menuName = "Guns/ShootConfiguration")]
public class ShootConfiguration : ScriptableObject
{
    public LayerMask hitMask;
    public Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float fireRate = 0.25f;
    public float damage = 10f;
}
