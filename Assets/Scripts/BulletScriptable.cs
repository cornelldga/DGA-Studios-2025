using UnityEngine;

[CreateAssetMenu(fileName = "BulletScriptable", menuName = "Bullets/BulletScriptable")]
public class BulletScriptable : ScriptableObject
{
    public float bulletSpeed;
    public float bulletLife;
    public float vertical;
    public float horizontal;
    public bool isCurved;
    public float angle;

}
