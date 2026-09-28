using UnityEngine;

public class PlayerPickupController : MonoBehaviour
{
    public float pickupRange = 3f;
    public float sphereRadius = 0.3f;
    public LayerMask gunMask;
    public PlayerInventory inventory;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TryPickup();
    }

    private void TryPickup()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.SphereCast(ray, sphereRadius, out RaycastHit hit, pickupRange, gunMask))
        {
            // Try gun pickup
            GunPickup gunPickup = hit.collider.GetComponent<GunPickup>();
            if (gunPickup != null)
            {
                gunPickup.OnPickup(inventory);
                return;
            }

            // Try grenade pickup
            GrenadePickup grenadePickup = hit.collider.GetComponent<GrenadePickup>();
            if (grenadePickup != null)
            {
                grenadePickup.OnPickup(inventory);
                return; 
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
