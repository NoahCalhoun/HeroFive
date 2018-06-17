using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5SystemBase
{
    public H5CharacterBase Owner { get; private set; }

    public virtual bool IsSystemValid { get { return true; } }

    public virtual void InitSystem(H5CharacterBase owner)
    {
        Owner = owner;
    }

    public virtual void UpdateSystem(float deltaTime)
    {

    }
}
