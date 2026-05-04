using Unity.VisualScripting;
using UnityEngine;

public class Point : MonoBehaviour
{
    public GameObject pointPrefab;

    //private float distSinceLastSpawn = 0f;
    private Vector3 SpawnPos;

    private void Start()
    {
   
    }

    public void DropSecondaryProjectile()
    {
        SpawnPos = this.transform.position;
        GameObject spawnedObject = Instantiate(pointPrefab, SpawnPos, Quaternion.identity);
        SetPrefabSettings(spawnedObject);
        Destroy(spawnedObject, 20f);
    }
    
    private void SetPrefabSettings(GameObject obj)
    {
        Bush bushScript = obj.GetComponent<Bush>();
        if (bushScript)
        {
            bushScript.setFire(true);
        }

        Dynamite DynaScript = obj.GetComponent<Dynamite>();
        if (DynaScript)
        {
            Granny granny = GetComponent<Granny>();
            Vector3 grannypos = granny.transform.position;
            StartCoroutine(DynaScript.ThrowRoutine(grannypos, SpawnPos));
        }
        
    }

    public void SetSecondaryPrefab(GameObject prefab)
    {
        this.pointPrefab = prefab;
    }
}