using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private static ResourceManager mInstance;
    public static ResourceManager Instance { get { if (mInstance == null) mInstance = new ResourceManager(); return mInstance; } }

    private Dictionary<string, Texture2D> mTextureDic = new Dictionary<string, Texture2D>();

    public Texture2D LoadImage()
    {
        var keyStr = "Test";
        if (mTextureDic.ContainsKey(keyStr))
            return mTextureDic[keyStr];

        var texture = Resources.Load("Texture/TestImage", typeof(Texture2D)) as Texture2D;
        if (texture == null)
            return null;

        mTextureDic.Add(keyStr, texture);
        return texture;
    }
}
