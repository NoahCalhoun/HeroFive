using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RESOURCE_TYPE
{
    None = -1,
    Character,
    Tile,
    Skill,
    Max
}

public class ResourceManager
{
    private static ResourceManager mInstance;
    public static ResourceManager Instance { get { if (mInstance == null) mInstance = new ResourceManager(); return mInstance; } }

    private Dictionary<string, Texture2D> mTextureDic = new Dictionary<string, Texture2D>();

    private string _WhereString;

    public Texture2D LoadImage(RESOURCE_TYPE _rType, string _keyStr)
    {
        if (_rType == RESOURCE_TYPE.None)
            return null;
        
        if (mTextureDic.ContainsKey(_keyStr))
            return mTextureDic[_keyStr];

        _WhereString = "Texture/FT2/";

        switch (_rType)
        {
            case RESOURCE_TYPE.Character:
                _WhereString += "Sprite/char_01a";
                break;
            case RESOURCE_TYPE.Tile:
                _WhereString += "Map/st51a";
                break;
            case RESOURCE_TYPE.Skill:
                _WhereString += "Sprite/";
                break;
        }
        var texture = Resources.Load(_WhereString, typeof(Texture2D)) as Texture2D;
        if (texture == null)
            return null;

        Resources.UnloadAsset(texture);
        mTextureDic.Add(_keyStr, texture);
        return texture;
        
    }
}
