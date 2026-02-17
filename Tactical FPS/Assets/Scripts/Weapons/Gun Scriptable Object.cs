using UnityEngine;

[CreateAssetMenu(fileName = "GunScriptableObject", menuName = "Guns/GunScriptableObject")]
public class GunScriptableObject : ScriptableObject
{
    public GunType Type;
    public string Name;
    public GameObject modelPrefab;
    public Vector3 spawnPoint, spawnRotation;

    public TrailConfiguration trailConfig;
    public ShootConfiguration shootConfig;

    [SerializeField] private AudioClip gunShotClip;
    public AudioClip GunShotClip => gunShotClip;

    public GameObject CreateModelInstance(Transform parent)
    {
        GameObject instance = Instantiate(modelPrefab, parent, false);
        instance.transform.localPosition = spawnPoint;
        instance.transform.localRotation = Quaternion.Euler(spawnRotation);
        return instance;
    }
}

    
