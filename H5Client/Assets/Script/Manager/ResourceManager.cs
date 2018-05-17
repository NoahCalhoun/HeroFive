using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private static ResourceManager mInstance;
    public static ResourceManager Instance { get { if (mInstance == null) mInstance = new ResourceManager(); return mInstance; } }

    private Dictionary<string, Texture2D> mTextureDic = new Dictionary<string, Texture2D>();

    public Texture2D LoadImage(string _keyStr, string _whereStr)
    {
        if (mTextureDic.ContainsKey(_keyStr))
            return mTextureDic[_keyStr];

        var texture = Resources.Load(_whereStr, typeof(Texture2D)) as Texture2D;
        if (texture == null)
            return null;

        Resources.UnloadAsset(texture);
        mTextureDic.Add(_keyStr, texture);
        return texture;
        
    }
}
