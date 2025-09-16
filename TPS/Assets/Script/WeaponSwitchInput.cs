using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WeaponSwitchInput : MonoBehaviour
{
    [SerializeField] private WeaponHolder holder;

    private void Awake()
    {
        if (!holder) holder = GetComponent<WeaponHolder>();
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.digit1Key.wasPressedThisFrame) holder?.SwitchTo(0);
            if (kb.digit2Key.wasPressedThisFrame) holder?.SwitchTo(1);
            if (kb.xKey.wasPressedThisFrame)      holder?.GoUnarmed();
            return;
        }
#endif
        if (Input.GetKeyDown(KeyCode.Alpha1)) holder?.SwitchTo(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) holder?.SwitchTo(1);
        if (Input.GetKeyDown(KeyCode.X))      holder?.GoUnarmed();
    }
}