using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;


public class FlowerSpawner : MonoBehaviour
{
    public int flowerCount = 10;

    public float spawnOffset = 1;

    public GameObject flowerPrefab;

    public Transform flowerHolder;

    public float flowerHeight = 5;

    public List<Vector3> flowerBulbPoints = new List<Vector3>();

    private List<GameObject> oldFlowers = new List<GameObject>();

    public float vaseWidth = 1;

    public float vaseHeight = 1;

    public float vaseHoleRadius = 1;

    public float flowerMassCenterSize = 1;

    public List<Material> matsToRanomize = new List<Material>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("PopulateFlowers")]
    public void PopulateFlowers()
    {
        foreach(GameObject go in oldFlowers)
        {
            DestroyImmediate(go);
        }

        flowerBulbPoints = GenerateBulbPoints();


        foreach (Vector3 v3 in flowerBulbPoints)
        {
            if (v3 != Vector3.up * flowerHeight)
                //if (Vector3.Distance(v3, Vector3.up * flowerHeight) > flowerCount / 30f)
            {
                GameObject thisFlower = Instantiate(flowerPrefab, flowerHolder);

                Vector3 stemPos = flowerHolder.parent.transform.position + (new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(0f, vaseWidth));

                Vector3 neckPos = flowerHolder.parent.transform.position + (new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(0f, vaseHoleRadius));

                thisFlower.transform.GetChild(0).transform.position = v3;

                thisFlower.transform.GetChild(1).transform.position = neckPos + (Vector3.up * vaseHeight);

                thisFlower.transform.GetChild(2).transform.position = stemPos + (Vector3.up * UnityEngine.Random.Range(0.001f, 0.1f));


                thisFlower.transform.GetChild(0).transform.LookAt(neckPos + (Vector3.up * (vaseHeight + UnityEngine.Random.Range(-0.1f, 0.1f))));

                thisFlower.transform.GetChild(0).transform.Rotate(new Vector3(-120,0,0), Space.Self);

                thisFlower.transform.GetChild(0).transform.Rotate(new Vector3(0, UnityEngine.Random.Range(-90f, 90f), 0), Space.Self);

                oldFlowers.Add(thisFlower);
            }


        }


        foreach(Material m in matsToRanomize)
        {
            m.SetFloat("_Rotation", UnityEngine.Random.Range(-180f, 180f));

            m.SetVector("_GenOffset", new Vector2(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f)));
        }

    }

    List<Vector3> GenerateBulbPoints()
    {
        List<Vector3> newBulbPoints = new List<Vector3>();

        newBulbPoints.Add(Vector3.up * flowerHeight);

        int flowersToSpawn = flowerCount;

        float flowerSpawnOffsetMulti = 0.0f;

        while (flowersToSpawn > 0)
        {
            Vector3 attemptPoint = (new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.1f, 40f), UnityEngine.Random.Range(-1f, 1f)).normalized * flowerSpawnOffsetMulti) + (Vector3.up * flowerHeight) + flowerHolder.transform.position;

            bool isTooClose = false;

            foreach(Vector3 v3 in newBulbPoints)
            {
                if (Vector3.Distance(v3, attemptPoint) < spawnOffset || Vector3.Distance((Vector3.up * flowerHeight) + flowerHolder.transform.position, attemptPoint) < flowerMassCenterSize)
                {
                    isTooClose = true;
                     
                    flowerSpawnOffsetMulti += 0.0001f;
                }
            }


            if (isTooClose == false)
            {
                newBulbPoints.Add(attemptPoint);

                 flowersToSpawn--;

                flowerSpawnOffsetMulti = 0.0f;
            }

        }


        return newBulbPoints;
    }
}
