using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public class ToolAttackTag : H5ObjectBase
{
    static StringBuilder sb = new StringBuilder();
    H5TileBase Owner;
    //UIWidget Root;
    UILabel Tag;

    public override void InitObject()
    {
    }

    public void InitTag(H5TileBase tile)
    {
        Owner = tile;
        Tag = GetComponentInChildren<UILabel>();
        ClearTag();
    }

    public void RefreshTag(SkillEditor.HitData_Tool data)
    {
        sb.Clear();

        sb.Append("[b]");

        switch (data.HitDirection)
        {
            case HitDirection_Tool.None: sb.Append('⊙'); break;
            case HitDirection_Tool.Up: sb.Append('↘'); break;
            case HitDirection_Tool.Down: sb.Append('↖'); break;
            case HitDirection_Tool.Left: sb.Append('↗'); break;
            case HitDirection_Tool.Right: sb.Append('↙'); break;
            case HitDirection_Tool.UpLeft: sb.Append('→'); break;
            case HitDirection_Tool.UpRight: sb.Append('↓'); break;
            case HitDirection_Tool.DownLeft: sb.Append('↑'); break;
            case HitDirection_Tool.DownRight: sb.Append('←'); break;
            case HitDirection_Tool.Random: sb.Append('※'); break;
        }

        sb.Append(data.HitValue);

        sb.Append("[/b]");

        Tag.text = sb.ToString();
    }

    public void ClearTag()
    {
        Tag.text = "";
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var _3dPos = Owner.TM.position + Vector3.up * /*Height*/ 0f;
        var projectPos = Camera.main.WorldToScreenPoint(_3dPos);
        var centerPos = Camera.main.WorldToScreenPoint(Camera.main.transform.position);
        centerPos.z = 0;
        TM.localPosition = projectPos - centerPos;
        Tag.depth = (int)(TM.localPosition.z * -1000f);
    }
}
