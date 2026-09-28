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
    public bool IsEquipped = false;
    private AudioSource audioSource;

    public int currentAmmo;
    public int ammoReserve;
    private bool isReloading = false;

    private ObjectPool<GameObject> impactPool;

    [Header("ADS Settings")]
    public Vector3 adsPosition;
    public Vector3 adsRotation;
    public float adsSpeed = 10f;

    private Vector3 hipfirePosition;
    private Quaternion hipfireRotation;

    private bool isAiming = false;
    private Transform holder;


    private void Start()
    {
        currentAmmo = data.clipSize;
        ammoReserve = data.maxAmmo;
    }

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.volume = 1f;

        shootsystem = GetComponentInChildren<ParticleSystem>();
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        // Set up the capped pool
        trailPool = new ObjectPool<TrailRenderer>(
            createFunc: CreateTrail,
            actionOnGet: trail =>
            {
                trail.gameObject.SetActive(true);
                trail.emitting = true;
            },
            actionOnRelease: trail =>
            {
                trail.emitting = false;
                trail.gameObject.SetActive(false);
            },
            actionOnDestroy: trail =>
            {
                Destroy(trail.gameObject);
            },
            collectionCheck: false, 
            defaultCapacity: data.trailConfig.poolSize,    // Optional: prewarm size
            maxSize:data.trailConfig.maxPoolSize             //Max number of trails in pool
        );

        //Hit VFX Pool
        impactPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(data.shootConfig.impactPrefab);
                obj.SetActive(false);
                return obj;
            },
            actionOnGet: obj =>
            {
                obj.SetActive(true);
            },
            actionOnRelease: obj =>
            {
                obj.SetActive(false);
            },
            actionOnDestroy: obj =>
            {
                Destroy(obj);
            },
            collectionCheck: false,
            defaultCapacity: data.shootConfig.impactPoolSize,
            maxSize: data.shootConfig.impactMaxPoolSize
            );
    }

    private void Update()
    {
        if (!IsEquipped || holder == null)
            return;

        Quaternion targetADS = Quaternion.Euler(adsRotation);
        Quaternion targetHipfire = hipfireRotation;

        if (isAiming)
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetADS,
                Time.deltaTime * adsSpeed
            );
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                adsPosition,
                Time.deltaTime * adsSpeed
            );
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetHipfire,
                Time.deltaTime * adsSpeed
            );
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                hipfirePosition,
                Time.deltaTime * adsSpeed
            );
        }

    }
    public void Initialize(Rigidbody rb)
    {
        lastShootTime = 0f;
    }

    public void Shoot()
    {
        if (!IsEquipped || isReloading) return;

        if (currentAmmo <= 0)
        {
            Debug.Log("No ammo — need to reload!");
            return;
        }

        if (Time.time > data.shootConfig.fireRate + lastShootTime)
        {
            lastShootTime = Time.time;

            currentAmmo--;
            InventoryUIController.Instance.UpdateAmmo(currentAmmo, ammoReserve);

            shootsystem.Play();
            //spawn gun audio
            audioSource.PlayOneShot(data.GunShotClip);

            Vector3 shootDirection = shootsystem.transform.forward
                + new Vector3(
                    Random.Range(-data.shootConfig.spread.x, data.shootConfig.spread.x),
                    Random.Range(-data.shootConfig.spread.y, data.shootConfig.spread.y),
                    Random.Range(-data.shootConfig.spread.z, data.shootConfig.spread.z)
                );
            shootDirection.Normalize();

            if (Physics.Raycast(shootsystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, data.shootConfig.hitMask))
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.TakeDamage(data.shootConfig.damage);

                //Spawn hit effect
                SpawnImpactEffect(hit);
                // Play trail to hit point
                StartCoroutine(PlayTrail(shootsystem.transform.position, hit.point, hit));
            }
            else
            {
                StartCoroutine(PlayTrail(shootsystem.transform.position, shootsystem.transform.position + (shootDirection * data.trailConfig.missDistance), new RaycastHit()));
            }
        }
    }

    public void Reload()
    {

        if (isReloading) return;
        if (currentAmmo == data.clipSize) return;  
        if (ammoReserve <= 0) return;                 

        isReloading = true;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(data.reloadTime);

        int needed = data.clipSize - currentAmmo;
        int reloadAmount = Mathf.Min(needed, ammoReserve);

        currentAmmo += reloadAmount;
        ammoReserve -= reloadAmount;

        isReloading = false;
        InventoryUIController.Instance.UpdateAmmo(currentAmmo, ammoReserve);
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
        instance.transform.SetParent(trailContainer); // parent to shared world container

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

    public void SetEquipped(bool value)
    {
        Debug.Log(gameObject.name + " Equipped = " + value);

        IsEquipped = value;
    }

    private void SpawnImpactEffect(RaycastHit hit)
    {
        if (data.shootConfig.impactPrefab == null) return;

        // Get pooled object
        GameObject impact = impactPool.Get();

        impact.transform.position = hit.point;
        impact.transform.rotation = Quaternion.LookRotation(hit.normal);

        // Optional: auto-despawn after effect finishes
        StartCoroutine(ReleaseImpactAfter(impact, 1f));
    }

    private IEnumerator ReleaseImpactAfter(GameObject impact, float time)
    {
        yield return new WaitForSeconds(time);
        impactPool.Release(impact);
    }

    public void InitializeADS(Transform weaponHolder)
    {
        holder = weaponHolder;

        // Read hipfire directly from weapon's current transform
        hipfirePosition = transform.localPosition;
        hipfireRotation = transform.localRotation;

        // Load ADS settings from scriptable object
        adsPosition = data.adsPosition;
        adsRotation = data.adsRotation;
        adsSpeed = data.adsSpeed;
    }
    public void SetADS(bool aiming)
    {
        isAiming = aiming;
    }

    public void ClearADS()
    {
        holder = null;
        isAiming = false; // ensure no rotation/position lerps continue
    }

}
