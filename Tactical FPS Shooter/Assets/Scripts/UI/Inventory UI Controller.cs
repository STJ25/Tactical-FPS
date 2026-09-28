using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;

    [Header("Panels")]
    public GameObject weaponPanel;
    public GameObject grenadePanel;

    [Header("Weapon UI")]
    public Image weaponIcon;
    public TMP_Text ammoText;

    [Header("Grenade UI")]
    public Image grenadeIcon;
    public TMP_Text grenadeCountText;

    private void Awake()
    {
        Instance = this;

        // Hide UI on start
        gameObject.SetActive(false);
        weaponPanel.SetActive(false);
        grenadePanel.SetActive(false);
    }

    public void ShowWeapon(GunScriptableObject gunData, int currentAmmo, int maxAmmo)
    {
        gameObject.SetActive(true);
        weaponPanel.SetActive(true);

        weaponIcon.sprite = gunData.gunIcon;
        ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    public void HideWeapon()
    {
        weaponPanel.SetActive(false);
    }

    public void ShowGrenade(Sprite icon, int count)
    {
        gameObject.SetActive(true);
        grenadePanel.SetActive(true);

        grenadeIcon.sprite = icon;
        grenadeCountText.text = "x" + count;
    }

    public void UpdateGrenadeCount(int count)
    {
        grenadeCountText.text = "x" + count;
    }

    public void HideGrenade()
    {
        grenadePanel.SetActive(false);
    }
}
