using System.Collections.Generic;
using UnityEngine;

//this makes ity so that only one component of this type can be added to a GameObject, in this case the player
[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    
    [Header("Input Settings")]
    [SerializeField]
    private KeyCode weaponCycleKey = KeyCode.E;

    [Header("Gun Settings")]
    [SerializeField]
    private GunType Gun;
    [SerializeField]
    private Transform GunParent;
    [SerializeField]
    private List<GunScriptableObject> Guns;

    private Dictionary<GunScriptableObject, GameObject> weaponModels = new();
    private int currentWeaponIndex = 0;
    //[SerializeField]
    //private PlayerIK inverseKinematics;

    [Header("Runtime Filled")]
    public GunScriptableObject activeGun;

    private void Start()
    {
        EquipWeapon(currentWeaponIndex);
        //inverse Kinematics Logic Here
    }

    private void Update()
    {
        // Number key weapon selection (1–9)
        for (int i = 0; i < Guns.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipWeapon(i);
                return;
            }
        }

        // Cycle weapon using custom key (e.g., E)
        if (Input.GetKeyDown(weaponCycleKey))
        {
            int nextIndex = (currentWeaponIndex + 1) % Guns.Count;
            EquipWeapon(nextIndex);
        }

        //Scroll wheel weapon cycling
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            int nextIndex = (currentWeaponIndex + 1) % Guns.Count;
            EquipWeapon(nextIndex);
        }
        else if (scroll < 0f)
        {
            int prevIndex = (currentWeaponIndex - 1 + Guns.Count) % Guns.Count;
            EquipWeapon(prevIndex);
        }
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= Guns.Count)
            return;

        // Deactivate previous weapon model
        if (activeGun != null && weaponModels.ContainsKey(activeGun))
        {
            weaponModels[activeGun].SetActive(false);
        }

        // Update selected gun
        currentWeaponIndex = index;
        activeGun = Guns[index];

        // If the model hasn't been spawned yet, spawn it now
        if (!weaponModels.ContainsKey(activeGun))
        {
            GameObject gunInstance = activeGun.CreateModelInstance(GunParent);
            GunBehaviour gunBehaviour = gunInstance.GetComponent<GunBehaviour>();

            if (gunBehaviour != null)
            {
                gunBehaviour.data = activeGun;
            }

            weaponModels[activeGun] = gunInstance;
        }

        // Show the selected gun
        weaponModels[activeGun].SetActive(true);
    }

    public GameObject GetActiveModel()
    {
        if (weaponModels.TryGetValue(activeGun, out var model))
            return model;
        return null;
    }

}
