using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

public class GunBehaviour : MonoBehaviour
{
    public GunScriptableObject data;
    private ParticleSystem shootsystem;
    private ObjectPool<TrailRenderer> trailPool;
    private float lastShootTime;
    private static Transform trailContainer;
    
    private void Awake()
    {
        shootsystem = GetComponentInChildren<ParticleSystem>();
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        // Set up the capped pool
        trailPool = new ObjectPool<TrailRenderer>(
            createFunc: CreateTrail,
            actionOnGet: trail => {
                trail.gameObject.SetActive(true);
                trail.emitting = true;
            },
            actionOnRelease: trail => {
                trail.emitting = false;
                trail.gameObject.SetActive(false);
            },
            actionOnDestroy: trail => {
                Destroy(trail.gameObject);
            },
            collectionCheck: false, // Set to true if you want debug warnings
            defaultCapacity: 70,    // Optional: prewarm size
            maxSize: 100             // ✅ LIMIT: Max number of trails in pool
        );

    }

    public void Initialize(Rigidbody rb)
    {
        lastShootTime = 0f;
    }

    public void Shoot()
    {
        if (Time.time > data.shootConfig.fireRate + lastShootTime)
        {
            lastShootTime = Time.time;
            shootsystem.Play();
            AudioSource.PlayClipAtPoint(data.GunShotClip, transform.position);

            // Bullet spread
            Vector3 shootDirection = shootsystem.transform.forward
                + new Vector3(
                    Random.Range(-data.shootConfig.spread.x, data.shootConfig.spread.x),
                    Random.Range(-data.shootConfig.spread.y, data.shootConfig.spread.y),
                    Random.Range(-data.shootConfig.spread.z, data.shootConfig.spread.z)
                );
            shootDirection.Normalize();

            if (Physics.Raycast(shootsystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, data.shootConfig.hitMask))
            {
                // ✅ Try to damage the object
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(data.shootConfig.damage); // Get damage value from SO
                }

                StartCoroutine(PlayTrail(shootsystem.transform.position, hit.point, hit));
            }
            else
            {
                StartCoroutine(PlayTrail(shootsystem.transform.position, shootsystem.transform.position + (shootDirection * data.trailConfig.missDistance), new RaycastHit()));
            }
        }
    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer trail = trailPool.Get();
        trail.gameObject.SetActive(true);
        trail.transform.position = startPoint;
        //yield return null;
        trail.Clear();
        yield return null;

        trail.emitting = true;
        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0f)
        {
            trail.transform.position = Vector3.Lerp(startPoint, endPoint, 1f - (remainingDistance / distance));
            remainingDistance -= data.trailConfig.SimulationSpeed * Time.deltaTime;
            yield return null;
        }

        trail.transform.position = endPoint;
        yield return new WaitForSeconds(data.trailConfig.duration);
        yield return null;

        trail.emitting = false;
        trail.gameObject.SetActive(false);
        trailPool.Release(trail);
    }

    private TrailRenderer CreateTrail()
    {
        // Create a global container once
        if (trailContainer == null)
        {
            GameObject container = new GameObject("TrailContainer");
            GameObject.DontDestroyOnLoad(container); // optional: keeps container across scenes
            trailContainer = container.transform;
        }

        GameObject instance = new GameObject("Bullet Trail");
        instance.transform.SetParent(trailContainer); // 🧼 parent to shared world container

        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = data.trailConfig.color;
        trail.material = data.trailConfig.material;
        trail.widthCurve = data.trailConfig.widthCurve;
        trail.time = data.trailConfig.duration;
        trail.minVertexDistance = data.trailConfig.minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

}
