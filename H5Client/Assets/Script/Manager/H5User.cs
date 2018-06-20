using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5User
{
    private static H5User mInstance;
    public static H5User Instance { get { if (mInstance == null) mInstance = new H5User(); return mInstance; } }
}
