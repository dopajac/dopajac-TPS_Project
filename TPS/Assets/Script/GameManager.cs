using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Bullet")]
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletPoint;
    [SerializeField] private float maxShootDelay = 0.2f;
    [SerializeField] private float currentShootDelay = 0.2f ;
    [SerializeField] private Text BulletCountText;
    private int maxBulletCount=30;
    private int currentBulletCount;
    
    [Header("Weapon FX")] 
    [SerializeField] private GameObject weaponFlashFX;
    [SerializeField] private Transform bulletCasePoint;
    [SerializeField] private GameObject bulletCaseFX;
    [SerializeField] private Transform weaponClipPoint;
    [SerializeField] private GameObject weaponClipFX;
    void Start()
    {
        instance = this;
        currentShootDelay = 0f;
        InitBullet();
    }

    // Update is called once per frame
    void Update()
    {
        BulletCountText.text = currentBulletCount + "/ " + maxBulletCount;;
    }

    public void Shooting(Vector3 targetPosition)
    {
        currentShootDelay += Time.deltaTime;
        if (currentShootDelay < maxShootDelay || currentBulletCount <= 0)
            return;

        currentBulletCount -= 1;
        currentShootDelay = 0;
        Instantiate(bulletCaseFX, bulletCasePoint);
        Instantiate(weaponFlashFX, bulletPoint);
        Vector3 aim = (targetPosition - bulletPoint.position).normalized;
        Instantiate(bulletPrefab, bulletPoint.position, Quaternion.LookRotation(aim, Vector3.up));
    }

    public void ReLoadClip()
    {
        Instantiate(weaponClipFX, weaponClipPoint);
        InitBullet();
    }

    private void InitBullet()
    {
        currentBulletCount = maxBulletCount;
    }

}
