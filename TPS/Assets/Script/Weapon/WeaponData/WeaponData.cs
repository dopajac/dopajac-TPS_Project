using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject prefab;
    public Vector3 previewRotation;
    public Vector3 previewScale = Vector3.one;
}
