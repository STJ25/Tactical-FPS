using UnityEngine;

[CreateAssetMenu(fileName = "TrailConfiguration", menuName = "Guns/TrailConfiguration")]
public class TrailConfiguration : ScriptableObject
{
    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.5f;
    public float minVertexDistance = 0.1f;
    public Gradient color;

    public float missDistance = 100f;
    public float SimulationSpeed = 100f;
}
