using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FoodStall : MonoBehaviour
{

    [SerializeField] private List<GameObject> prefabList;

    private List<Transform> stallPosList = new List<Transform>();

    private Dictionary<Transform, GameObject> stallDictionary = new Dictionary<Transform, GameObject>();


    void Awake()
    {
        GenerateStallDictionary();
    }

    private void GenerateStallDictionary()
    {
        Transform positions = transform.Find("StallPositions");

        foreach (Transform pos in positions)
        {
            stallPosList.Add(pos);
        }


        for (int i = 0; i < stallPosList.Count; i++)
        {
            if (prefabList[i] == null) { continue; }

            stallDictionary[stallPosList[i]] = prefabList[i];
            //Debug.Log(stallDictionary[stallPosList[i]]);
        }
        
    }


}
