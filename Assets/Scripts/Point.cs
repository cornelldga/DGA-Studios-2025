using UnityEngine;

public class Point : MonoBehaviour
{
    public GameObject pointPrefab;

    //private float distSinceLastSpawn = 0f;
    //private Vector3 lastPos;

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
            Destroy(spawnedObject, 10f);
            distSinceLastSpawn -= spawnInterval;
        }
    }

    private void SetPrefabSettings(GameObject obj)
    {
        Bush bushScript = obj.GetComponent<Bush>();
        if (bushScript)
        {
            bushScript.setFire(true);
        }
    }

    public void SetPointPrefab(GameObject prefab)
    {
        this.pointPrefab = prefab;
    }
}