using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum OBJECT_TYPE
{
    OBJECT_TEST,
    OBJECT_TILE
}

public class WorldManager : MonoBehaviour
{
    private Transform WorldRoot;

    // Use this for initialization
    void Start()
    {
        WorldRoot = GameObject.FindGameObjectWithTag("World").transform;

        SpawnTestObject(-2, -2, "Prefab/TestObject", OBJECT_TYPE.OBJECT_TEST);
        SpawnTestObject(2, 2, "Prefab/TestObject", OBJECT_TYPE.OBJECT_TEST);
        SpawnTestObject(0, 0, "Prefab/Tile", OBJECT_TYPE.OBJECT_TILE);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public H5ObjectBase SpawnTestObject(float x, float z, string objwhere, OBJECT_TYPE objtype)
    {
        var testObjectPrefab = Resources.Load(objwhere) as GameObject;
        var testObject = GameObject.Instantiate(testObjectPrefab);

        if (testObject == null)
            return null;

        H5ObjectBase h5Test = null;

        switch (objtype)
        {
            case OBJECT_TYPE.OBJECT_TEST:
                h5Test = testObject.AddComponent<H5TestObject>();
                break;

            case OBJECT_TYPE.OBJECT_TILE:
                h5Test = testObject.AddComponent<H5TileBase>();
                break;
        }

        if (h5Test == null)
            return null;

        h5Test.TM.SetParent(WorldRoot);
        h5Test.InitObject();
        h5Test.PlaceOnWorld(x, z);

        return h5Test;
    }

}
