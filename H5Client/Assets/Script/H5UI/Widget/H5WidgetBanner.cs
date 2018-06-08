using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5WidgetBanner : H5ObjectBase
{
    H5CharacterBase Owner;
    float Height;
    UITexture BannerTexture;

    public override void InitObject()
    {
    }

    public void InitBanner(H5CharacterBase owner)
    {
        Owner = owner;
        Height = 1.5f;
        BannerTexture = GetComponent<UITexture>();
    }

    // Update is called once per frame
    void Update()
    {
        var _3dPos = Owner.TM.position + Vector3.up * Height;
        var projectPos = Camera.main.WorldToScreenPoint(_3dPos);
        var centerPos = Camera.main.WorldToScreenPoint(Camera.main.transform.position);
        centerPos.z = 0;
        TM.localPosition = projectPos - centerPos;
        BannerTexture.depth = (int)(TM.localPosition.z * -1000f);
    }
}
