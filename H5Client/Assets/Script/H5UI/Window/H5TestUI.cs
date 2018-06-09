using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5TestUI : H5WindowBase
{
    public class H5TestUIData : H5WindowDataBase
    {
        public override UIWindowType Type { get { return UIWindowType.TestUI; } }

        public CharacterType CharacterType;
    }

    public UITexture Texture;
    public UIEventListener TextureListener;
    bool TestBool;

    public Texture2D FirstTexture;
    public Texture2D SecondTexture;

    public override UIWindowType Type { get { return UIWindowType.TestUI; } }

    void Awake()
    {
        TextureListener.onClick = OnClickTexture;
        TestBool = true;

        FirstTexture = Resources.Load("Texture/FT2/Image/vis03a", typeof(Texture2D)) as Texture2D;
        SecondTexture = Resources.Load("Texture/FT2/Image/vis03b", typeof(Texture2D)) as Texture2D;
    }

    void OnClickTexture(GameObject obj)
    {
        TestBool = !TestBool;

        Texture.mainTexture = TestBool ? FirstTexture : SecondTexture;
    }

    public override void OnOpenWindow()
    {
        TestBool = true;
    }

    public override void OnCloseWindow()
    {
    }

    public override void SetWindowData(H5WindowDataBase data)
    {
        if (data.Type != Type)
            return;

        var windowData = data as H5TestUIData;
        if (windowData == null)
            return;

        switch(windowData.CharacterType)
        {
            case CharacterType.Monarch:
                FirstTexture = Resources.Load("Texture/FT2/Image/face01a", typeof(Texture2D)) as Texture2D;
                SecondTexture = Resources.Load("Texture/FT2/Image/face01c", typeof(Texture2D)) as Texture2D;
                break;

            case CharacterType.Tanker:
                FirstTexture = Resources.Load("Texture/FT2/Image/face02a", typeof(Texture2D)) as Texture2D;
                SecondTexture = Resources.Load("Texture/FT2/Image/face02c", typeof(Texture2D)) as Texture2D;
                break;

            case CharacterType.Dealer:
                FirstTexture = Resources.Load("Texture/FT2/Image/face04a", typeof(Texture2D)) as Texture2D;
                SecondTexture = Resources.Load("Texture/FT2/Image/face04n", typeof(Texture2D)) as Texture2D;
                break;

            case CharacterType.Positioner:
                FirstTexture = Resources.Load("Texture/FT2/Image/face05a", typeof(Texture2D)) as Texture2D;
                SecondTexture = Resources.Load("Texture/FT2/Image/face05c", typeof(Texture2D)) as Texture2D;
                break;

            case CharacterType.Supporter:
                FirstTexture = Resources.Load("Texture/FT2/Image/face03a", typeof(Texture2D)) as Texture2D;
                SecondTexture = Resources.Load("Texture/FT2/Image/face03m", typeof(Texture2D)) as Texture2D;
                break;

            case CharacterType.Monster:
                FirstTexture = Resources.Load("Texture/FT2/Image/face25a", typeof(Texture2D)) as Texture2D;
                SecondTexture = Resources.Load("Texture/FT2/Image/face25b", typeof(Texture2D)) as Texture2D;
                break;

            default:
                FirstTexture = Resources.Load("Texture/FT2/Image/vis03a", typeof(Texture2D)) as Texture2D;
                SecondTexture = Resources.Load("Texture/FT2/Image/vis03b", typeof(Texture2D)) as Texture2D;
                break;
        }

        Texture.mainTexture = FirstTexture;
    }
}
