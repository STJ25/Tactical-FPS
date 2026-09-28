using UnityEngine;

[CreateAssetMenu(fileName = "TrailConfiguration", menuName = "Guns/TrailConfiguration")]
public class TrailConfiguration : ScriptableObject
{
    [Header("Trail Settings")]
    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.5f;
    public float minVertexDistance = 0.1f;
    public Gradient color;
    public float missDistance = 100f;
    public float SimulationSpeed = 100f;

    [Header("Trail Pool Settings")]
    public int poolSize = 70;
    public int maxPoolSize = 100;
}
