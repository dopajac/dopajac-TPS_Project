using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Refs")]
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Text bulletCountText;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!currentWeapon || !bulletCountText) return;
        bulletCountText.text = currentWeapon.CurrentAmmo + " / " + currentWeapon.MagCapacity;
    }

    // 총구(origin=총구)에서 발사 (기존 호환)
    public void Shooting(Vector3 targetPosition)
    {
        if (!currentWeapon) return;
        currentWeapon.TryShoot(targetPosition);
    }

    // fromCamera=true 이면 카메라 중앙(origin)에서 발사
    public void Shooting(Vector3 targetPosition, bool fromCamera, Vector3 origin)
    {
        if (!currentWeapon) return;

        if (fromCamera)
            currentWeapon.TryShootFrom(origin, targetPosition);
        else
            currentWeapon.TryShoot(targetPosition);
    }

    public void ReLoadClip()
    {
        if (!currentWeapon) return;
        currentWeapon.Reload();
    }

    public void SetCurrentWeapon(Weapon w)
    {
        currentWeapon = w;
    }

    public Weapon GetCurrentWeapon() => currentWeapon;
}