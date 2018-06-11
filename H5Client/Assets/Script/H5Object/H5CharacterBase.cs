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

public class H5CharacterBase : H5ObjectBase
{
    public CharacterType Type { get; protected set; }

    MovementSystem m_MovementSystem;

    public override void InitObject()
    {
    }

    public void InitCharacter(CharacterType type)
    {
        Type = type;

        m_MovementSystem = new MovementSystem();
        m_MovementSystem.InitSystem(this);

        var loadMaterial = Resources.Load("Material/Character") as Material;
        var renderer = GetComponentInChildren<MeshRenderer>();
        renderer.material = loadMaterial;

        switch(Type)
        {
            case CharacterType.Tanker:
                renderer.material.color = new Color32(0, 0, 255, 255);
                break;

            case CharacterType.Dealer:
                renderer.material.color = new Color32(255, 0, 255, 255);
                break;

            case CharacterType.Positioner:
                renderer.material.color = new Color32(128, 128, 128, 255);
                break;

            case CharacterType.Supporter:
                renderer.material.color = new Color32(0, 255, 0, 255);
                break;

            case CharacterType.Monarch:
                renderer.material.color = new Color32(255, 255, 0, 255);
                break;

            case CharacterType.Monster:
                renderer.material.color = new Color32(255, 0, 0, 255);
                break;
        }

        var bannerPrefab = Resources.Load("Prefab/Widget/Banner") as GameObject;
        var banner = GameObject.Instantiate(bannerPrefab);
        banner.GetComponent<H5WidgetBanner>().InitBanner(this);
        banner.transform.SetParent(UIManager.Instance.UICameraRoot);
        banner.transform.localScale = Vector3.one;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_MovementSystem != null) m_MovementSystem.Update();
    }

    public void MoveTo(byte _x, byte _y)
    {
        m_MovementSystem.CalcMovePath(_x, _y);
    }
}
