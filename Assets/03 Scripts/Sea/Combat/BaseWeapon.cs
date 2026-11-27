using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    protected GameObject _owner;
    public virtual bool CanFire => false;

    public virtual void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    public virtual void Equip() { }
    public virtual void Unequip() { }

    public virtual void BeginAttack() { }
    public virtual void EndAttack() { }

    public virtual void Fire(Vector2 direction) { }
}
