using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public PlayerInventory inventory;
    public Transform playerRoot;

    void Start()
    {
        if (inventory == null) Debug.LogError("inventory not assigned on PlayerShooter");
        inventory.Initialize(playerRoot);
    }

    void Update()
    {
        HandleFire();
        HandleDrop();
        HandleWeaponSwitching();
        HandleADS();


        //throw grenade
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (inventory.activeSlot == PlayerInventory.ActiveSlot.Grenade &&
                inventory.EquippedGrenade != null)
            {
                ThrowGrenade();
            }
        }

    }

    private void HandleFire()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (inventory.activeSlot == PlayerInventory.ActiveSlot.Primary && inventory.PrimaryGun != null)
                inventory.PrimaryGun.Reload();

            if (inventory.activeSlot == PlayerInventory.ActiveSlot.Secondary && inventory.SecondaryGun != null)
                inventory.SecondaryGun.Reload();
        }

        // Only the active slot can fire
        if (inventory.activeSlot == PlayerInventory.ActiveSlot.Primary && inventory.PrimaryGun != null)
        {
            if (Input.GetMouseButton(0))
                inventory.PrimaryGun.Shoot();
            return;
        }

        if (inventory.activeSlot == PlayerInventory.ActiveSlot.Secondary && inventory.SecondaryGun != null)
        {
            if (Input.GetMouseButton(0))
                inventory.SecondaryGun.Shoot();
            return;
        }

        if (inventory.activeSlot == PlayerInventory.ActiveSlot.Grenade && inventory.EquippedGrenade != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Grenade fire placeholder (implement throw later)");
            }
        }

    }

    private void HandleDrop()
    {
        //Debug.Log("HandleDrop called");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q detected — calling DropActiveWeapon()");
            inventory.DropActiveWeapon();
        }
    }

    private void HandleWeaponSwitching()
    {
        // Number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
            inventory.EquipActiveWeapon(PlayerInventory.ActiveSlot.Primary);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            inventory.EquipActiveWeapon(PlayerInventory.ActiveSlot.Secondary);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            inventory.EquipActiveWeapon(PlayerInventory.ActiveSlot.Grenade);

        // Scroll Wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            SwitchNext();
        else if (scroll < 0f)
            SwitchPrevious();
    }

    private void SwitchNext()
    {
        if (inventory.activeSlot == PlayerInventory.ActiveSlot.Primary)
            inventory.EquipActiveWeapon(PlayerInventory.ActiveSlot.Secondary);
        else if (inventory.activeSlot == PlayerInventory.ActiveSlot.Secondary)
            inventory.EquipActiveWeapon(PlayerInventory.ActiveSlot.Grenade);
        else
            inventory.EquipActiveWeapon(PlayerInventory.ActiveSlot.Primary);
    }

    private void SwitchPrevious()
    {
        if (inventory.activeSlot == PlayerInventory.ActiveSlot.Grenade)
            inventory.EquipActiveWeapon(PlayerInventory.ActiveSlot.Secondary);
        else if (inventory.activeSlot == PlayerInventory.ActiveSlot.Secondary)
            inventory.EquipActiveWeapon(PlayerInventory.ActiveSlot.Primary);
        else
            inventory.EquipActiveWeapon(PlayerInventory.ActiveSlot.Grenade);
    }

    private void ThrowGrenade()
    {
        GrenadeBehaviour grenade = inventory.EquippedGrenade;
        if (grenade == null) return;

        // Remove from inventory WITHOUT enabling physics or applying drop force
        inventory.RemoveGrenadeFromInventory(grenade);

        // Now actually throw it (GrenadeBehaviour.Throw will enable physics and apply forces)
        grenade.Throw(playerRoot);

        // Auto switch to next weapon
        inventory.AutoEquipNext();
    }

    private void HandleADS()
    {
        if (inventory.activeSlot == PlayerInventory.ActiveSlot.Primary &&
            inventory.PrimaryGun != null)
        {
            inventory.PrimaryGun.SetADS(Input.GetMouseButton(1));
        }
        else if (inventory.activeSlot == PlayerInventory.ActiveSlot.Secondary &&
                 inventory.SecondaryGun != null)
        {
            inventory.SecondaryGun.SetADS(Input.GetMouseButton(1));
        }
    }

}
