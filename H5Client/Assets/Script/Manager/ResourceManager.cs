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
    public static SpriteData GetSpriteData(this Texture2D texture, string name)
    {
        return ResourceManager.Instance.GetSpriteData(texture, name);
    }
}

public class SpriteData
{
    private Vector2 TextureSize;
    private Vector2 SpriteSize;
    private Vector2 SpriteOffset;
    private Vector2 SpritePivot;

    public Vector4 UV { get; private set; }
    public Color Cutoff { get; private set; }
    public Vector4 UVSprite { get; private set; }
    public Vector4 ShaderSetUV { get; private set; }
    public Vector4 ShaderTestUV { get; private set; }

    public SpriteData(Sprite sprite, float meshPixel = 200f, float pivotX = 0.5f, float pivotY = 0.5f)
    {
        if (sprite == null)
            return;

        TextureSize = new Vector2(sprite.texture.width, sprite.texture.height);
        SpriteSize = new Vector2(sprite.rect.xMax - sprite.rect.xMin, sprite.rect.yMax - sprite.rect.yMin);
        SpriteOffset = new Vector2(sprite.rect.xMin, sprite.rect.yMin);
        SpritePivot = new Vector2(sprite.pivot.x, sprite.pivot.y);

        UV = new Vector4(SpriteSize.x / TextureSize.x, SpriteSize.y / TextureSize.y, SpriteOffset.x / TextureSize.x, SpriteOffset.y / TextureSize.y);
        Cutoff = sprite.texture.GetPixel(0, 0);
        UVSprite = new Vector4(meshPixel / SpriteSize.x, meshPixel / SpriteSize.y
            , (pivotX - SpritePivot.x / meshPixel) * meshPixel / TextureSize.x
            , (pivotY - SpritePivot.y / meshPixel) * meshPixel / TextureSize.y);

        ShaderSetUV = new Vector4(UV.x * UVSprite.x, UV.y * UVSprite.y, UV.z - UVSprite.z, UV.w - UVSprite.w);
        ShaderTestUV = new Vector4(UV.z, UV.x + UV.z, UV.w, UV.y + UV.w);
    }
}

public class ResourceManager
{
    private static ResourceManager mInstance;
    public static ResourceManager Instance { get { if (mInstance == null) mInstance = new ResourceManager(); return mInstance; } }

    private Dictionary<string, Texture2D> mTextureDic = new Dictionary<string, Texture2D>();
    private Dictionary<Texture2D, Dictionary<string, SpriteData>> mSpriteDic = new Dictionary<Texture2D, Dictionary<string, SpriteData>>();

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
            LoadSpriteData(sb.ToString(), texture);
            return texture;
        }

        sb.Replace(ImageFolder, MapFolder);
        texture = Resources.Load(sb.ToString(), typeof(Texture2D)) as Texture2D;
        if (texture != null)
        {
            mTextureDic.Add(key, texture);
            LoadSpriteData(sb.ToString(), texture);
            return texture;
        }

        sb.Replace(MapFolder, SpriteFolder);
        texture = Resources.Load(sb.ToString(), typeof(Texture2D)) as Texture2D;
        if (texture != null)
        {
            mTextureDic.Add(key, texture);
            LoadSpriteData(sb.ToString(), texture);
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
            UnloadSpriteData(mTextureDic[key]);
            Resources.UnloadAsset(mTextureDic[key]);
            mTextureDic[key] = null;
            mTextureDic.Remove(key);
            return true;
        }

        return false;
    }

    void LoadSpriteData(string path, Texture2D texture)
    {
        if (mSpriteDic.ContainsKey(texture))
            return;

        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        if (sprites == null || sprites.Length <= 0)
            return;

        //var spriteTexture = sprites[0].texture;
        mSpriteDic.Add(texture, new Dictionary<string, SpriteData>());
        for (int i = 0; i < sprites.Length; ++i)
        {
            mSpriteDic[texture].Add(sprites[i].name, new SpriteData(sprites[i]));
            //Resources.UnloadAsset(sprites[i]);
            //sprites[i] = null;
        }
        //Resources.UnloadAsset(spriteTexture);
        //spriteTexture = null;
    }

    void UnloadSpriteData(Texture2D texture)
    {
        if (mSpriteDic.ContainsKey(texture) == false || mSpriteDic[texture].Count <= 0)
            return;

        var e = mSpriteDic[texture].GetEnumerator();
        //while(e.MoveNext())
        //{
        //    mSpriteDic[texture][e.Current.Key] = null;
        //}
        mSpriteDic[texture].Clear();
        mSpriteDic.Remove(texture);
    }

    public SpriteData GetSpriteData(Texture2D texture, string name)
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
