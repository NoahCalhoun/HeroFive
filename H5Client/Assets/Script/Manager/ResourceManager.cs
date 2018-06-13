using System.Text;
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

public enum Episode
{
    FT1,
    FT2,
}

public class ResourceManager
{
    private static ResourceManager mInstance;
    public static ResourceManager Instance { get { if (mInstance == null) mInstance = new ResourceManager(); return mInstance; } }

    private Dictionary<string, Texture2D> mTextureDic = new Dictionary<string, Texture2D>();

    StringBuilder sb = new StringBuilder();

    readonly string TextureFolder = "Texture/";
    readonly string ImageFolder = "/Image/";
    readonly string MapFolder = "/Map/";
    readonly string SpriteFolder = "/Sprite/";

    public Texture2D LoadTexture(Episode ep, string name)
    {
        sb.Clear();
        sb.Append(ep.ToString());
        sb.Append("_");
        sb.Append(name);
        string key = sb.ToString();

        if (mTextureDic.ContainsKey(key))
        {
            return mTextureDic[key];
        }

        Texture2D texture;

        sb.Clear();
        sb.Append(TextureFolder);
        sb.Append(ep.ToString());
        sb.Append(ImageFolder);
        sb.Append(name);

        texture = Resources.Load(sb.ToString(), typeof(Texture2D)) as Texture2D;
        if (texture != null)
        {
            mTextureDic.Add(key, texture);
            return texture;
        }

        sb.Replace(ImageFolder, MapFolder);
        texture = Resources.Load(sb.ToString(), typeof(Texture2D)) as Texture2D;
        if (texture != null)
        {
            mTextureDic.Add(key, texture);
            return texture;
        }

        sb.Replace(MapFolder, SpriteFolder);
        texture = Resources.Load(sb.ToString(), typeof(Texture2D)) as Texture2D;
        if (texture != null)
        {
            mTextureDic.Add(key, texture);
            return texture;
        }

        return null;
    }

    public bool UnloadTexture(Episode ep, string name)
    {
        sb.Clear();
        sb.Append(ep.ToString());
        sb.Append("_");
        sb.Append(name);
        string key = sb.ToString();

        if (mTextureDic.ContainsKey(key))
        {
            Resources.UnloadAsset(mTextureDic[key]);
            mTextureDic[key] = null;
            mTextureDic.Remove(key);
            return true;
        }

        return false;
    }

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
