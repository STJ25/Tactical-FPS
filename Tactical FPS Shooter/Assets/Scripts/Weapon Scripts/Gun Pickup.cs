using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GunBehaviour gunBehaviour;

    private void Reset()
    {
        gunBehaviour = GetComponent<GunBehaviour>();
    }

    public void OnPickup(PlayerInventory inventory)
    {
        // Prevent picking up guns already held by the player
        if (gunBehaviour.IsEquipped)
            return;

        // Disable physics
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Add gun into inventory (will parent and deactivate)
        inventory.EquipGun(gunBehaviour);
    }
}
