using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
        // Keep only global systems here (score, pause, level flow, etc.)
        // Weapons/Ammo/UI are handled by WeaponHolder/PlayerManager.
    }
}