using UnityEngine;

public class BushTrail : MonoBehaviour
{
    [SerializeField] public GameObject bushPrefab;
    [SerializeField] private float bushSpawnInterval = 0.7f;

    private float distSinceLastBush = 0f;
    private Vector3 lastPos;

    private void Start()
    {
        lastPos = transform.position;
    }

    private void Update()
    {
        float distTravelled = Vector3.Distance(transform.position, lastPos);
        distSinceLastBush += distTravelled;
        lastPos = transform.position;

        while (distSinceLastBush >= bushSpawnInterval)
        {
            GameObject bush = Instantiate(bushPrefab, transform.position, Quaternion.identity);
            bush.GetComponent<Bush>().setFire(true);
            Destroy(bush, 10f);
            distSinceLastBush -= bushSpawnInterval;
        }
    }

    public void SetBushPrefab(GameObject prefab)
    {
        bushPrefab = prefab;
    }
}