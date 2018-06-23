using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingWindow : H5WindowBase
{
    public override UIWindowType Type { get { return UIWindowType.Loading; } }

    public UITexture LoadingImage;
    public UILabel LoaingProgress;
    public UILabel LoadingComplete;

    float LoadingPercent;
    bool Tween;
    float TweenTime;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Tween == false)
            return;

        TweenTime += (Time.deltaTime * 4f);
        LoadingComplete.gameObject.SetActive(((int)TweenTime) % 2 == 1);
    }

    public override void OnOpenWindow()
    {
        LoadingPercent = 0;
        Tween = false;
        LoadingComplete.gameObject.SetActive(false);

        var list = H5Table.LoadingScene.GetIDList();
        var idx = Random.Range(0, list.Count);

        var loadingTextureData = H5Table.LoadingScene.GetDataByID(list[idx]);
        if (loadingTextureData == null)
            return;

        var loadingTexture = ResourceManager.Instance.LoadTexture((Episode)System.Enum.Parse(typeof(Episode), loadingTextureData.EP), loadingTextureData.TEXTURE);
        if (loadingTexture == null)
            return;

        LoadingImage.mainTexture = loadingTexture;
        LoadingImage.MakePixelPerfect();

        LoaingProgress.text = "0%";
    }

    public void SetPercent(float percent)
    {
        LoadingPercent = percent;
        LoaingProgress.text = string.Format("{0}%", (int)(percent * 100));

        if (percent >= 1f)
            Tween = true;
    }
}
