using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class H5WindowBase : H5ObjectBase
{
    public abstract UIWindowType Type { get; }

    public override void InitObject()
    {
    }

    public virtual void OnOpenWindow()
    {

    }

    public virtual void OnCloseWindow()
    {

    }
}