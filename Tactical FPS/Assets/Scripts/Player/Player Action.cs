using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector gunSelector;

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed && gunSelector.activeGun != null)
        {
            GameObject activeModel = gunSelector.GetActiveModel();
            if (activeModel != null && activeModel.TryGetComponent(out GunBehaviour behaviour))
            {
                behaviour.Shoot();
            }

        }
    }
}
