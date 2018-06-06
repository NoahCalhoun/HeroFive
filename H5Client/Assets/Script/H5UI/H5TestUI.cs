using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5TestUI : H5WindowBase
{
    public UITexture Texture;
    public UIEventListener TextureListener;
    bool TestBool;

    void Start()
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
}
