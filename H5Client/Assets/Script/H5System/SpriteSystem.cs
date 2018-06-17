using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState
{
    Idle,
    Move,
    Skill,
    Hit,
    Faint
}

public class SpriteSet
{
    public Texture2D Texture;
    public List<KeyValuePair<float, KeyValuePair<string, string>>> SpriteName = new List<KeyValuePair<float, KeyValuePair<string, string>>>();

    public bool Loop = true;
    public float Scale = 1f;
    private float ElapsedTime;
    private float TotalTime;

    public KeyValuePair<string, string> Advance(float deltaTime)
    {
        ElapsedTime += deltaTime * Scale;

        if (Loop)
        {
            while (ElapsedTime >= TotalTime)
                ElapsedTime -= TotalTime;
        }
        else
        {
            ElapsedTime = Mathf.Min(ElapsedTime, TotalTime);
        }

        float curTime = ElapsedTime;
        for (int i = 0; i < SpriteName.Count; ++i)
        {
            curTime -= SpriteName[i].Key;
            if (curTime <= 0f)
                return SpriteName[i].Value;
        }

        return SpriteName[SpriteName.Count - 1].Value;
    }

    public void CalcTotalTime()
    {
        TotalTime = 0;
        for (int i = 0; i < SpriteName.Count; ++i)
        {
            TotalTime += SpriteName[i].Key;
        }
        TotalTime = Mathf.Max(0, TotalTime);
    }
}

public class SpriteSystem : H5SystemBase
{
    public ActionState State { get; set; }

    private Dictionary<ActionState, SpriteSet> mSpriteDic = new Dictionary<ActionState, SpriteSet>(EnumComparer<ActionState>.Instance);

    private MeshRenderer OwnerRenderer;

    public override bool IsSystemValid { get { return OwnerRenderer != null; } }

    public override void InitSystem(H5CharacterBase owner)
    {
        base.InitSystem(owner);

        OwnerRenderer = owner.GO.GetComponentInChildren<MeshRenderer>();
        State = ActionState.Idle;
    }

    public bool AddSet(ActionState state, SpriteSet set)
    {
        if (mSpriteDic.ContainsKey(state))
            return false;

        mSpriteDic.Add(state, set);
        return true;
    }

    public override void UpdateSystem(float deltaTime)
    {
        if (mSpriteDic.ContainsKey(State) == false)
            return;

        var spriteName = Owner.Direction.IsFront() ? mSpriteDic[State].Advance(deltaTime).Key : mSpriteDic[State].Advance(Time.deltaTime).Value;
        
        var sprite = mSpriteDic[State].Texture.GetSpriteData(spriteName);
        OwnerRenderer.material.SetTexture("_MainTex", mSpriteDic[State].Texture);
        OwnerRenderer.material.SetColor("_CutoffColor", sprite.Cutoff);
        OwnerRenderer.material.SetVector("_SetUV", sprite.ShaderSetUV);
        OwnerRenderer.material.SetVector("_TestUV", sprite.ShaderTestUV);
        OwnerRenderer.material.SetInt("_Mirror", Owner.Direction.IsMirror() ? 1 : 0);
    }
}
