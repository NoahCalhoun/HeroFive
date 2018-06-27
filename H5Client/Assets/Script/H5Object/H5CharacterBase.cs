using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum CharacterType
{
    None = -1,
    Tanker,
    Dealer,
    Positioner,
    Supporter,
    Monarch,
    Monster
}

public class H5CharacterBase : H5ObjectBase, IMovable
{
    public CharacterType Type { get; protected set; }
    public H5Direction Direction;

    public MovementSystem MovementSystem { get; private set; }
    public SpriteSystem SpriteSystem { get; private set; }
    public StatSystem StatSystem { get; private set; }
    public SkillSystem SkillSystem { get; private set; }

    public H5TileBase OwnTile;

    public override void InitObject()
    {
    }

    public void InitCharacter(int tableId)
    {
        var charData = H5Table.Character.GetDataByID(tableId);

        if (charData == null)
            return;

        Type = CharacterType.Monarch;
        Direction = H5Direction.Right;

        MovementSystem = new MovementSystem();
        MovementSystem.InitSystem(this);

        SpriteSystem = new SpriteSystem();
        SpriteSystem.InitSystem(this);

        StatSystem = new StatSystem();
        StatSystem.InitSystem(this);

        SkillSystem = new SkillSystem();
        SkillSystem.InitSystem(this);

        var loadMaterial = Resources.Load("Material/Character") as Material;
        var renderer = GetComponentInChildren<MeshRenderer>();
        renderer.material = loadMaterial;
        renderer.gameObject.transform.localScale = new Vector3(4, 4, 1);
        renderer.gameObject.transform.localRotation = WorldManager.Instance != null ? WorldManager.Instance.CameraRoot.rotation : Camera.main.transform.rotation;

        StatSystem.HP = charData.HP;
        StatSystem.ATK = charData.ATK;
        StatSystem.SPD = charData.SPD;
        StatSystem.LCM = charData.LCM;
        StatSystem.EDR = charData.EDR;

        SpriteSet spriteData;
        var spriteTableData = H5Table.Sprite.GetDataByNAME(charData.SPRITE);
        for (int i = 0; i < spriteTableData.ACTION_.Length; ++i)
        {
            if (spriteTableData.ACTION_[i] == "0")
                break;

            var spriteSetData = H5Table.SpriteSet.GetDataByNAME(spriteTableData.SPRITE_SET_[i]);
            if (spriteSetData == null)
                continue;

            spriteData = new SpriteSet();
            spriteData.Texture = ResourceManager.Instance.LoadTexture((Episode)(Enum.Parse(typeof(Episode), spriteSetData.EP)), spriteSetData.TEXTURE);
            for (int j = 0; j < spriteSetData.SPRITE_L_.Length; ++j)
            {
                if (spriteSetData.SPRITE_L_[j] == "0")
                    break;

                spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(
                    spriteSetData.TIME_[j] * 0.001f, new KeyValuePair<string, string>(spriteSetData.SPRITE_L_[j], spriteSetData.SPRITE_R_[j])));
            }
            spriteData.CalcTotalTime();
            SpriteSystem.AddSet((ActionState)(Enum.Parse(typeof(ActionState), spriteTableData.ACTION_[i])), spriteData);
        }
        SpriteSystem.UpdateSystem(0);
    }

    public void InitCharacter(CharacterType type)
    {
        Type = type;
        Direction = H5Direction.Right;

        MovementSystem = new MovementSystem();
        MovementSystem.InitSystem(this);

        SpriteSystem = new SpriteSystem();
        SpriteSystem.InitSystem(this);

        StatSystem = new StatSystem();
        StatSystem.InitSystem(this);

        SkillSystem = new SkillSystem();
        SkillSystem.InitSystem(this);

        var loadMaterial = Resources.Load("Material/Character") as Material;
        var renderer = GetComponentInChildren<MeshRenderer>();
        renderer.material = loadMaterial;
        renderer.gameObject.transform.localScale = new Vector3(4, 4, 1);
        renderer.gameObject.transform.localRotation = WorldManager.Instance.CameraRoot.rotation;

        float idleTerm = 0.2f;
        switch (Type)
        {
            case CharacterType.Tanker:
                {
                    var spriteData = new SpriteSet();
                    spriteData.Texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_01a");
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle00", "Idle00")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle01")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle02", "Idle02")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle01")));
                    spriteData.CalcTotalTime();
                    SpriteSystem.AddSet(ActionState.Idle, spriteData);
                    break;
                }

            case CharacterType.Dealer:
                {
                    var spriteData = new SpriteSet();
                    spriteData.Texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_03a");
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle00", "Idle00")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle01")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle02", "Idle02")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle01")));
                    spriteData.CalcTotalTime();
                    SpriteSystem.AddSet(ActionState.Idle, spriteData);
                    break;
                }

            case CharacterType.Positioner:
                {
                    var spriteData = new SpriteSet();
                    spriteData.Texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_04a");
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle00", "Idle00")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle01")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle02", "Idle02")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle01")));
                    spriteData.CalcTotalTime();
                    SpriteSystem.AddSet(ActionState.Idle, spriteData);
                    break;
                }

            case CharacterType.Supporter:
                {
                    var spriteData = new SpriteSet();
                    spriteData.Texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_02a");
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle00", "Idle00")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle01")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle02", "Idle02")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle01")));
                    spriteData.CalcTotalTime();
                    SpriteSystem.AddSet(ActionState.Idle, spriteData);
                    break;
                }

            case CharacterType.Monarch:
                {
                    var spriteData = new SpriteSet();
                    spriteData.Texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_00a");
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle00", "Idle03")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle04")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle02", "Idle05")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle04")));
                    spriteData.CalcTotalTime();
                    SpriteSystem.AddSet(ActionState.Idle, spriteData);

                    spriteData = new SpriteSet();
                    spriteData.Texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_00b");
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(0.1f, new KeyValuePair<string, string>("Move00", "Move04")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(0.1f, new KeyValuePair<string, string>("Move01", "Move05")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(0.1f, new KeyValuePair<string, string>("Move02", "Move06")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(0.1f, new KeyValuePair<string, string>("Move03", "Move07")));
                    spriteData.CalcTotalTime();
                    SpriteSystem.AddSet(ActionState.Move, spriteData);

                    spriteData = new SpriteSet();
                    spriteData.Texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_00a");
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Hit00", "Hit01")));
                    spriteData.CalcTotalTime();
                    SpriteSystem.AddSet(ActionState.Hit, spriteData);
                    break;
                }

            case CharacterType.Monster:
                {
                    var spriteData = new SpriteSet();
                    spriteData.Texture = ResourceManager.Instance.LoadTexture(Episode.FT2, "char_00a");
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle00", "Idle03")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle04")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle02", "Idle05")));
                    spriteData.SpriteName.Add(new KeyValuePair<float, KeyValuePair<string, string>>(idleTerm, new KeyValuePair<string, string>("Idle01", "Idle04")));
                    spriteData.CalcTotalTime();
                    SpriteSystem.AddSet(ActionState.Idle, spriteData);
                    break;
                }
        }

        var bannerPrefab = Resources.Load("Prefab/Widget/Banner") as GameObject;
        var banner = GameObject.Instantiate(bannerPrefab);
        banner.GetComponent<H5WidgetBanner>().InitBanner(this);
        banner.transform.SetParent(UIManager.Instance.UICameraRoot);
        banner.transform.localScale = Vector3.one;

        SpriteSystem.UpdateSystem(0);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (MovementSystem != null) MovementSystem.Update();

        if (SpriteSystem != null)
            SpriteSystem.UpdateSystem(Time.deltaTime);
    }

    public void Move(byte x, byte y)
    {
        MovementSystem.SetWalk(x, y);
    }

    public void KnockBack(H5Direction dir, byte count)
    {
        MovementSystem.SetKnockBack(dir, count);
    }
}
