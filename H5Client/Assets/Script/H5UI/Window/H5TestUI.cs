using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5TestUI : H5WindowBase
{
    public UITexture Texture;
    public UIEventListener TextureListener;
    bool TestBool;

    public override UIWindowType Type { get { return UIWindowType.TestUI; } }

    void Awake()
    {
        TextureListener.onClick = OnClickTexture;
        TestBool = true;
    }

    void OnClickTexture(GameObject obj)
    {
        TestBool = !TestBool;

        Texture.mainTexture = TestBool ?
            Resources.Load("Texture/FT2/Image/vis03a", typeof(Texture2D)) as Texture2D
            : Resources.Load("Texture/FT2/Image/vis03b", typeof(Texture2D)) as Texture2D;
    }

    public override void OnOpenWindow()
    {
        Texture.mainTexture = Resources.Load("Texture/FT2/Image/vis03a", typeof(Texture2D)) as Texture2D;
        TestBool = true;
    }

    public override void OnCloseWindow()
    {
    }
}
