using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A player projectile that fires multiple bullets in a spread
/// </summary>
public class Whiskey : Base
{
    [Tooltip("The number of additional bullets fired")]
    [SerializeField] int extraBullets;
    [SerializeField] float angleChange;

    bool original = true;
    public override void Start()
    {
        base.Start();
        float evenAngle = (extraBullets % 2 == 0) ? .5f : 0;
        transform.Rotate(new Vector3(0, 0, .5f * angleChange), Space.Self);
        float angle = (Mathf.Floor(extraBullets / 2) + evenAngle) * -angleChange;
        if (!original) return;
        for (int i = 0; i < extraBullets/2; i++)
        {
            Whiskey proj = Instantiate(this, transform.position, transform.rotation);
            proj.original = false;
            proj.transform.Rotate(new Vector3(0, 0, angle), Space.Self);
            angle += angleChange;
        }
        angle += angleChange;
        for (int i = extraBullets / 2; i < extraBullets; i++)
        {
            Whiskey proj = Instantiate(this, transform.position, transform.rotation);
            proj.original = false;
            proj.transform.Rotate(new Vector3(0, 0, angle), Space.Self);
            angle += angleChange;
        }
    }
}
