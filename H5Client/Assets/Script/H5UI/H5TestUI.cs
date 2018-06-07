using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5TestUI : H5WindowBase
{
    public UITexture Texture;
    public UIEventListener TextureListener;
    bool TestBool;

    public override UIWindowType Type { get { return UIWindowType.TestUI; } }

    void Start()
    {
        TextureListener.onClick = OnClickTexture;
        TestBool = true;
        Texture.mainTexture = Resources.Load("Texture/FT2/Image/face01a", typeof(Texture2D)) as Texture2D;
    }

    void OnClickTexture(GameObject obj)
    {
        TestBool = !TestBool;

        //Texture.mainTexture = TestBool ?
        //    Resources.Load("Texture/FT2/Image/vis03a", typeof(Texture2D)) as Texture2D
        //    : Resources.Load("Texture/FT2/Image/vis03b", typeof(Texture2D)) as Texture2D;
        Texture.mainTexture = TestBool ?
            Resources.Load("Texture/FT2/Image/face01a", typeof(Texture2D)) as Texture2D
            : Resources.Load("Texture/FT2/Image/face01c", typeof(Texture2D)) as Texture2D;
    }
}
