using UnityEngine;

[CreateAssetMenu(fileName = "BulletScriptable", menuName = "Bullets/BulletScriptable")]
public class BulletScriptable : ScriptableObject
{
    public float bulletSpeed;
    public float vertical;
    public float horizontal;
    public float bulletLife;
    public Rigidbody2D rb;
    public bool isCurved;
    public float angle;

}
