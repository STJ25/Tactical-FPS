using UnityEngine;

[CreateAssetMenu(fileName = "GunScriptableObject", menuName = "Guns/GunScriptableObject")]
public class GunScriptableObject : ScriptableObject
{
    [Header("Gun Settings")]
    public GunType Type;
    public string Name;

    [Header("Ammo Settings")]
    public int clipSize = 30;      // How many bullets per mag
    public int maxAmmo = 90;       // Ammo remaining in reserve
    public float reloadTime = 1.8f; // Reload duration (customizable)

    [Header("ADS Settings")]
    public Vector3 adsPosition;      // local position relative to holder
    public Vector3 adsRotation;      // local rotation relative to holder (Euler angles)
    public float adsSpeed = 10f;     // how fast the weapon moves into ADS

    [Header("References")]
    public TrailConfiguration trailConfig;
    public ShootConfiguration shootConfig;
    public Sprite gunIcon;  
    [SerializeField] private AudioClip gunShotClip;
    public AudioClip GunShotClip => gunShotClip;


}

    
