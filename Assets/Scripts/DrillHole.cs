using UnityEngine;

/// <summary>
/// Represents a single hole in the ground object caused from Drill boss entering and exiting states
/// 
/// </summary>
public class DrillHole : Obstacle
{
    // if this hole is initialized exiting underground
    private bool exitHole;

    [Header("Rock Splash Damage Settings")]
    [SerializeField] private float radius = 3f;
    [SerializeField] private int debrisCount = 10;

    void Start()
    {
        base.Start();
        // TODO: Should the emersion rock debris damage be called here?
    }

    void Update()
    {
        base.Update();
        
        // if dynamite explosion, call Deactivate
        // Deactivate();
    }
}
