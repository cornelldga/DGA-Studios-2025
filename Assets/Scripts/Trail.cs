using UnityEngine;

public class Trail : MonoBehaviour
{
    private GameObject trailPrefab;
    // Lifetime of trail objects, negative to not destroy.
    private float trailLifetime = 0;
    [SerializeField] private float spawnInterval = 0.7f;

    private float distSinceLastSpawn = 0f;
    private Vector3 lastPos;

    GameObject newBoss;

    private void Start()
    {
        lastPos = transform.position;
    }

    private void Update()
    {
        float distTravelled = Vector3.Distance(transform.position, lastPos);
        distSinceLastSpawn += distTravelled;
        lastPos = transform.position;

        while (distSinceLastSpawn >= spawnInterval)
        {
            GameObject spawnedObject = Instantiate(trailPrefab, transform.position, Quaternion.identity);
            SetPrefabSettings(spawnedObject);

            //if it is a bush set the sprite for ash
            if (spawnedObject.GetComponent<Bush>() != null)
            {
                spawnedObject.GetComponent<Bush>().overrideAsh(newBoss);
            }
            if (trailLifetime >= 0)
            {
                Destroy(spawnedObject, trailLifetime);
            }
            distSinceLastSpawn -= spawnInterval;
        }
    }

    private void SetPrefabSettings(GameObject obj)
    {
        Bush bushScript = obj.GetComponent<Bush>();
        SmokePellet smokeScript = obj.GetComponent<SmokePellet>();
        DynamiteExplosion dynamiteScript = obj.GetComponent<DynamiteExplosion>();
        if (bushScript)
        {
            bushScript.setFire(true);
        } else if (smokeScript)
        {
            smokeScript.setSmokeLength(14, 2.5f);
        } else if (dynamiteScript)
        {
            dynamiteScript.changeImpulse(0.1f);
        }
    }

    public void SetTrailPrefab(GameObject prefab)
    {
        this.trailPrefab = prefab;
    }
    public void SetTrailLifetime(float lifetime)
    {
        this.trailLifetime = lifetime;
    }

    public void SetBoss(GameObject boss)
    {
        newBoss = boss;
    }
}