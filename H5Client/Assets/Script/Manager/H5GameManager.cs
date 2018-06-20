using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5GameManager : MonoBehaviour
{
    private static H5GameManager mInstance;
    public static H5GameManager Instance { get { if (mInstance == null) mInstance = GameObject.FindGameObjectWithTag("Game").GetComponent<H5GameManager>(); return mInstance; } }
    
    void Start()
    {

    }
    
    void Update()
    {

    }
}
