using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Bullet")]
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletPoint;
    void Start()
    {
        instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shooting(Vector3 targetPosition)
    {
        Vector3 aim = (targetPosition - bulletPoint.position).normalized;
        Instantiate(bulletPrefab, bulletPoint.position, Quaternion.LookRotation(aim, Vector3.up));
    }
}
