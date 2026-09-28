using UnityEngine;

[CreateAssetMenu(fileName = "ShootConfiguration", menuName = "Guns/ShootConfiguration")]
public class ShootConfiguration : ScriptableObject
{
    [Header("Layer and Recoil Settings")]
    public LayerMask hitMask;
    public Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);

    [Header("Rate and Damage Settings")]
    public float fireRate = 0.25f;
    public float damage = 10f;

    [Header("Impact Settings")]
    public GameObject impactPrefab;
    public int impactPoolSize = 20;
    public int impactMaxPoolSize = 40;

}
