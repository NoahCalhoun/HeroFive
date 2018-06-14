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

public class SpriteData
{
    public Texture2D Texture;
    public List<KeyValuePair<Sprite, Sprite>> Sprites;
}

public class SpriteSystem : H5SystemBase
{
    public ActionState State { get; private set; }

    private Dictionary<ActionState, SpriteData> mSpriteDic = new Dictionary<ActionState, SpriteData>(EnumComparer<ActionState>.Instance);
}
