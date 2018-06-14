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

public static class ResourceHelper
{
    public static Sprite GetSprite(this Texture2D texture, string name)
    {
        return ResourceManager.Instance.GetSprite(texture, name);
    }
}

public class ResourceManager
{
    private static ResourceManager mInstance;
    public static ResourceManager Instance { get { if (mInstance == null) mInstance = new ResourceManager(); return mInstance; } }

    private Dictionary<string, Texture2D> mTextureDic = new Dictionary<string, Texture2D>();
    private Dictionary<Texture2D, Dictionary<string, Sprite>> mSpriteDic = new Dictionary<Texture2D, Dictionary<string, Sprite>>();

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
            LoadSprite(sb.ToString(), texture);
            return texture;
        }

        sb.Replace(ImageFolder, MapFolder);
        texture = Resources.Load(sb.ToString(), typeof(Texture2D)) as Texture2D;
        if (texture != null)
        {
            mTextureDic.Add(key, texture);
            LoadSprite(sb.ToString(), texture);
            return texture;
        }

        sb.Replace(MapFolder, SpriteFolder);
        texture = Resources.Load(sb.ToString(), typeof(Texture2D)) as Texture2D;
        if (texture != null)
        {
            mTextureDic.Add(key, texture);
            LoadSprite(sb.ToString(), texture);
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
            UnloadSprite(mTextureDic[key]);
            Resources.UnloadAsset(mTextureDic[key]);
            mTextureDic[key] = null;
            mTextureDic.Remove(key);
            return true;
        }

        return false;
    }

    void LoadSprite(string path, Texture2D texture)
    {
        if (mSpriteDic.ContainsKey(texture))
            return;

        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        if (sprites == null || sprites.Length <= 0)
            return;

        mSpriteDic.Add(texture, new Dictionary<string, Sprite>());
        for (int i = 0; i < sprites.Length; ++i)
        {
            mSpriteDic[texture].Add(sprites[i].name, sprites[i]);
        }
    }

    void UnloadSprite(Texture2D texture)
    {
        if (mSpriteDic.ContainsKey(texture) == false)
            return;

        var e = mSpriteDic[texture].GetEnumerator();
        while(e.MoveNext())
        {
            mSpriteDic[texture][e.Current.Key] = null;
        }
        mSpriteDic[texture].Clear();
        mSpriteDic.Remove(texture);
    }

    public Sprite GetSprite(Texture2D texture, string name)
    {
        if (mSpriteDic.ContainsKey(texture) == false)
            return null;

        if (mSpriteDic[texture].ContainsKey(name) == false)
            return null;

        return mSpriteDic[texture][name];
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
