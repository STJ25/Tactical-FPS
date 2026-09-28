using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("Holder References")]
    public Transform weaponHolderPrimary;
    public Transform weaponHolderSecondary;
    public Transform weaponHolderGrenade;

    private Transform playerTransform;

    [Header("Equipped Weapons")]
    [SerializeField] private GunBehaviour primaryGun;
    [SerializeField] private GunBehaviour secondaryGun;

    [SerializeField] private List<GrenadeBehaviour> grenades = new List<GrenadeBehaviour>();
    public GrenadeBehaviour EquippedGrenade { get; private set; }
    public int maxGrenades = 2;

    public GunBehaviour PrimaryGun => primaryGun;
    public GunBehaviour SecondaryGun => secondaryGun;

    [Header("Active Weapon Slot")]
    public ActiveSlot activeSlot = ActiveSlot.None;

    public enum ActiveSlot
    {
        None,
        Primary,
        Secondary,
        Grenade
    }

    public void Initialize(Transform player)
    {
        playerTransform = player;
    }

    public void EquipActiveWeapon(ActiveSlot slot)
    {
        activeSlot = slot;

        // disable all visuals
        if (primaryGun != null) primaryGun.gameObject.SetActive(false);
        if (secondaryGun != null) secondaryGun.gameObject.SetActive(false);
        if (EquippedGrenade != null) EquippedGrenade.gameObject.SetActive(false);

        // unequip all
        if (primaryGun != null) primaryGun.SetEquipped(false);
        if (secondaryGun != null) secondaryGun.SetEquipped(false);
        if (EquippedGrenade != null) EquippedGrenade.SetEquipped(false);

        switch (slot)
        {
            case ActiveSlot.Primary:
                if (primaryGun != null)
                {

                    primaryGun.gameObject.SetActive(true);
                    primaryGun.SetEquipped(true);
                    DisableCollider(primaryGun);

                    primaryGun.InitializeADS(weaponHolderPrimary);

                    InventoryUIController.Instance.ShowWeapon(
                        primaryGun.data,
                        primaryGun.currentAmmo,
                        primaryGun.ammoReserve
                    );
                }
                break;


            case ActiveSlot.Secondary:
                if (secondaryGun != null)
                {

                    secondaryGun.gameObject.SetActive(true);
                    secondaryGun.SetEquipped(true);
                    DisableCollider(secondaryGun);

                    secondaryGun.InitializeADS(weaponHolderSecondary);

                    InventoryUIController.Instance.ShowWeapon(
                        secondaryGun.data,
                        secondaryGun.currentAmmo,
                        secondaryGun.ammoReserve
                    );
                }
                break;


            case ActiveSlot.Grenade:
                if (EquippedGrenade != null)
                {
                    EquippedGrenade.gameObject.SetActive(true);
                    EquippedGrenade.SetEquipped(true);
                    DisableCollider(EquippedGrenade);

                    // update UI
                    InventoryUIController.Instance.ShowGrenade(
                        EquippedGrenade.data.grenadeIcon,
                        grenades.Count
                    );
                }
                break;

            case ActiveSlot.None:
                InventoryUIController.Instance.HideWeapon();
                InventoryUIController.Instance.HideGrenade();
                break;
        }
    }


    private void DisableCollider(MonoBehaviour obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    public void EquipGun(GunBehaviour gun)
    {
        switch (gun.data.Type)
        {
            case GunType.Pistol:
                EquipSecondary(gun);
                break;

            case GunType.Rifle:
                EquipPrimary(gun);
                break;

            default:
                EquipPrimary(gun);
                break;
        }
    }

    public void EquipGrenade(GrenadeBehaviour grenade)
    {
        if (grenades.Count >= maxGrenades)
        {
            DropGrenade(grenade, playerTransform);
            return;
        }

        grenades.Add(grenade);
        EquippedGrenade = grenade;

        grenade.transform.SetParent(weaponHolderGrenade);
        grenade.transform.localPosition = Vector3.zero;
        grenade.transform.localRotation = Quaternion.identity;

        grenade.SetEquipped(true);   // disable physics properly
        grenade.gameObject.SetActive(true); // <-- THIS WAS THE REAL MISSING LINE

        // NOW hide visually
        grenade.gameObject.SetActive(false);

        EquipActiveWeapon(ActiveSlot.Grenade);

        InventoryUIController.Instance.ShowGrenade(
            grenade.data.grenadeIcon,
            grenades.Count
        );
    }

    public void RemoveGrenadeFromInventory(GrenadeBehaviour grenade)
    {
        if (!grenades.Contains(grenade)) return;

        grenades.Remove(grenade);

        if (EquippedGrenade == grenade)
            EquippedGrenade = grenades.Count > 0 ? grenades[grenades.Count - 1] : null;

        // Detach from holder so Throw() can set parent/physics
        grenade.transform.SetParent(null);

        // Keep grenade kinematic until Throw enables physics
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        // Update UI
        if (InventoryUIController.Instance != null)
        {
            InventoryUIController.Instance.UpdateGrenadeCount(grenades.Count);
            if (grenades.Count == 0)
                InventoryUIController.Instance.HideGrenade();
        }
    }

    private void EquipPrimary(GunBehaviour gun)
    {
        if (primaryGun != null)
        {
            DropWeapon(gun, playerTransform);
            return;
        }

        primaryGun = gun;
        gun.gameObject.SetActive(false);

        gun.transform.SetParent(weaponHolderPrimary);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;

        EquipActiveWeapon(ActiveSlot.Primary);
        InventoryUIController.Instance.ShowWeapon(
            gun.data,
            gun.currentAmmo,
            gun.ammoReserve
            );

    }

    private void EquipSecondary(GunBehaviour gun)
    {
        if (secondaryGun != null)
        {
            DropWeapon(gun, playerTransform);
            return;
        }

        secondaryGun = gun;
        gun.gameObject.SetActive(false);

        gun.transform.SetParent(weaponHolderSecondary);
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;

        EquipActiveWeapon(ActiveSlot.Secondary);

        InventoryUIController.Instance.ShowWeapon(
            gun.data,
            gun.currentAmmo,
            gun.ammoReserve
            );
    }

    public void DropActiveWeapon()
    {
        switch (activeSlot)
        {
            case ActiveSlot.Primary:
                if (primaryGun != null)
                {
                    GunBehaviour drop = primaryGun;
                    primaryGun = null;
                    DropWeapon(drop, playerTransform);
                    AutoEquipNext();
                }
                break;

            case ActiveSlot.Secondary:
                if (secondaryGun != null)
                {
                    GunBehaviour drop = secondaryGun;
                    secondaryGun = null;
                    DropWeapon(drop, playerTransform);
                    AutoEquipNext();
                }
                break;

            case ActiveSlot.Grenade:
                if (EquippedGrenade != null)
                {
                    GrenadeBehaviour g = EquippedGrenade;
                    DropGrenade(g, playerTransform);
                    AutoEquipNext();
                }
                break;
        }
    }

    public void DropGrenade(GrenadeBehaviour grenade, Transform player)
    {
        if (!grenades.Contains(grenade)) return;

        grenades.Remove(grenade);

        if (EquippedGrenade == grenade)
            EquippedGrenade = grenades.Count > 0 ? grenades[grenades.Count - 1] : null;

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        Collider col = grenade.GetComponent<Collider>();

        grenade.transform.SetParent(null);

        grenade.SetEquipped(false);

        rb.isKinematic = false;
        rb.detectCollisions = true;
        col.enabled = true;

        grenade.transform.position += Vector3.up * 0.3f;

        rb.AddForce(player.forward * 2f + player.up * 1f, ForceMode.Impulse);

        InventoryUIController.Instance.UpdateGrenadeCount(grenades.Count);

        if (grenades.Count == 0)
            InventoryUIController.Instance.HideGrenade();

    }

    private void DropWeapon(GunBehaviour gun, Transform player)
    {
        gun.SetEquipped(false);
        gun.ClearADS();

        Rigidbody rb = gun.GetComponent<Rigidbody>();
        Collider col = gun.GetComponent<Collider>();

        gun.transform.SetParent(null);

        rb.isKinematic = false;
        rb.detectCollisions = true;
        col.enabled = true;

        gun.transform.position += Vector3.up * 0.3f;

        rb.AddForce(player.forward * 3f + player.up * 1.5f, ForceMode.Impulse);

        InventoryUIController.Instance.HideWeapon();

    }

    public void AutoEquipNext()
    {
        if (secondaryGun != null)
        {
            EquipActiveWeapon(ActiveSlot.Secondary);
            return;
        }

        if (primaryGun != null)
        {
            EquipActiveWeapon(ActiveSlot.Primary);
            return;
        }

        if (grenades.Count > 0)
        {
            EquippedGrenade = grenades[grenades.Count - 1];
            EquipActiveWeapon(ActiveSlot.Grenade);
            return;
        }

        EquipActiveWeapon(ActiveSlot.None);
    }

    public void AddAmmo(int amount)
    {
        // If rifle is active, add to rifle
        if (activeSlot == ActiveSlot.Primary && primaryGun != null)
        {
            primaryGun.ammoReserve += amount;
            InventoryUIController.Instance.UpdateAmmo(primaryGun.currentAmmo, primaryGun.ammoReserve);
            return;
        }

        // If pistol is active, add to pistol
        if (activeSlot == ActiveSlot.Secondary && secondaryGun != null)
        {
            secondaryGun.ammoReserve += amount;
            InventoryUIController.Instance.UpdateAmmo(secondaryGun.currentAmmo, secondaryGun.ammoReserve);
            return;
        }

        // If grenade active → no ammo
        if (activeSlot == ActiveSlot.Grenade)
            return;

        // If nothing equipped → add ammo evenly to both
        if (primaryGun != null)
            primaryGun.ammoReserve += amount;

        if (secondaryGun != null)
            secondaryGun.ammoReserve += amount;
    }

}
