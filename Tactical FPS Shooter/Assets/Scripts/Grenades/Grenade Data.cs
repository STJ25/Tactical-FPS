using UnityEngine;

[CreateAssetMenu(fileName = "NewGrenadeData", menuName = "Grenades/Grenade Data")]
public class GrenadeData : ScriptableObject
{
    [Header("Grenade Settings")]
    public float throwForce = 15f;
    public float upwardForce = 4f;
    public float fuseTime = 2.5f;
    public int maxCarry = 2;

    [Header("Physics")]
    public float explosionForce = 500f;
    public float explosionUpwardModifier = 1.5f;

    [Header("Explosion Settings")]
    public GameObject explosionEffect;
    public float explosionRadius = 4f;
    public float explosionDamage = 80f;

    [Header("References")]
    [SerializeField] private AudioClip explosionSound;
    public AudioClip ExplosionSound => explosionSound;
    public Sprite grenadeIcon;
}
