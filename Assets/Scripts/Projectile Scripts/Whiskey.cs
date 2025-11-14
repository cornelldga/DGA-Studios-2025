using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A player projectile that fires multiple bullets in a spread
/// </summary>
public class Whiskey : Base
{
    [SerializeField] int extraBullets;
    [SerializeField] float angleChange;

    bool original = true;
    public override void Start()
    {
        base.Start();
        float angle = angleChange;
        if (!original) return;
        for (int i = 0; i < extraBullets; i++)
        {
            if (i % 2 == 0)
            {
                float newAngle = transform.rotation.eulerAngles.z - angleChange;
                Quaternion newRotation = Quaternion.Euler(0, 0, newAngle);
                Whiskey proj = Instantiate(this, transform.position, newRotation);
                proj.original = false;
            }
            if (i % 2 == 1)
            {
                float newAngle = transform.rotation.eulerAngles.z + angleChange;
                Quaternion newRotation = Quaternion.Euler(0, 0, newAngle);
                Whiskey proj = Instantiate(this, transform.position, newRotation);
                angle += angleChange;
                proj.original = false;
            }
        }
    }
}
