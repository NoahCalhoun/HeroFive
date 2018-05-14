using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private Transform WorldRoot;

    // Use this for initialization
    void Start()
    {
        WorldRoot = GameObject.FindGameObjectWithTag("World").transform;

        SpawnTestObject(-2, -2);
        SpawnTestObject(2, 2);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public H5TestObject SpawnTestObject(float x, float z)
    {
        var testObjectPrefab = Resources.Load("Prefab/TestObject") as GameObject;
        var testObject = GameObject.Instantiate(testObjectPrefab);
        if (testObject == null)
            return null;

        var h5Test = testObject.AddComponent<H5TestObject>();
        h5Test.TM.SetParent(WorldRoot);
        h5Test.InitObject();
        h5Test.PlaceOnWorld(x, z);

        return h5Test;
    }
}
