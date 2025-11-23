using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBergGenerator : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnArea;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private List<int> icebergPoolIds;
    [SerializeField] private float spawnInterval = 7f;
    [SerializeField] private int maxIcebergsPerSpawn = 5;
    [SerializeField] private float minDistanceBetweenSpawns = 5.0f;

    [Header("Initial Generation Settings")]
    [SerializeField] private int initialAmountOfIcebergs = 10;
    [SerializeField] private Vector2 customBounds = Vector2.zero;

    private void Start()
    {
        FirstGeneration();
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnIceBergs();
        }
    }

    void FirstGeneration()
    {
        if (icebergPoolIds.Count == 0) return;

        List<Vector3> usedPositions = new List<Vector3>();

        //Always spawn one iceberg at the center
        int centerPoolId = icebergPoolIds[Random.Range(0, icebergPoolIds.Count)];
        GameObject centerIceberg = ObjectPooler.Instance.SpawnFromPool(centerPoolId, Vector3.zero, Quaternion.identity);
        if (centerIceberg != null && spawnParent != null)
            centerIceberg.transform.SetParent(spawnParent);

        usedPositions.Add(Vector3.zero);

        //Determine bounds
        Vector3 areaSize = new Vector3(customBounds.x * 2f, 0f, customBounds.y * 2f);
        Vector3 areaCenter = Vector3.zero;


        //Spawn additional icebergs
        for (int i = 0; i < initialAmountOfIcebergs; i++)
        {
            Vector3 spawnPos;
            int attempts = 0;

            do
            {
                float x = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
                float z = Random.Range(-areaSize.z / 2f, areaSize.z / 2f);
                spawnPos = areaCenter + new Vector3(x, 0f, z);
                attempts++;
            } while (IsTooClose(spawnPos, usedPositions) && attempts < 10);

            usedPositions.Add(spawnPos);

            int poolId = icebergPoolIds[Random.Range(0, icebergPoolIds.Count)];
            GameObject iceberg = ObjectPooler.Instance.SpawnFromPool(poolId, spawnPos, Quaternion.identity);
            if (iceberg != null && spawnParent != null)
                iceberg.transform.SetParent(spawnParent);
        }
    }

    void SpawnIceBergs()
    {
        if (spawnArea == null || icebergPoolIds.Count == 0) return;

        Vector3 areaSize = spawnArea.localScale;
        Vector3 areaCenter = spawnArea.position;

        int icebergCount = Random.Range(1, maxIcebergsPerSpawn + 1);
        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < icebergCount; i++)
        {
            Vector3 spawnPos;
            int attempts = 0;

            //Find a unique spawn position
            do
            {
                float x = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
                float z = Random.Range(-areaSize.z / 2f, areaSize.z / 2f);
                spawnPos = areaCenter + new Vector3(x, 0f, z);
                attempts++;
            } while (IsTooClose(spawnPos, usedPositions) && attempts < 10);

            usedPositions.Add(spawnPos);

            int poolId = icebergPoolIds[Random.Range(0, icebergPoolIds.Count)];
            GameObject iceberg = ObjectPooler.Instance.SpawnFromPool(poolId, spawnPos, Quaternion.identity);
            if (iceberg != null && spawnParent != null)
            {
                iceberg.transform.SetParent(spawnParent);
            }
        }
    }

    bool IsTooClose(Vector3 pos, List<Vector3> others)
    {
        foreach (var other in others)
        {
            if (Vector3.Distance(pos, other) < minDistanceBetweenSpawns)
                return true;
        }
        return false;
    }
}