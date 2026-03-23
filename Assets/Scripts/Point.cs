using UnityEngine;

public class Point : MonoBehaviour
{
    public GameObject pointPrefab;

    //private float distSinceLastSpawn = 0f;
    private Vector3 SpawnPos;

    private void Start()
    {
        //SpawnPos = transform.position;
    }

    private void Update()
    {
           /* GameObject spawnedObject = Instantiate(pointPrefab, SpawnPos, Quaternion.identity);
            SetPrefabSettings(spawnedObject);
            Destroy(spawnedObject, 10f);
            distSinceLastSpawn -= spawnInterval; */
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