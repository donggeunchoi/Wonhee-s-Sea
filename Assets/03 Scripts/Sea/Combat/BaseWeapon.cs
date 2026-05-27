using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    protected GameObject _owner;
    protected PlayerRoot _playerRoot;

    public virtual void SetOwner(GameObject owner)
    {
        _owner = owner;
        _playerRoot = owner != null ? owner.GetComponent<PlayerRoot>() : null;
    }

    public virtual void Equip() { }
    public virtual void Unequip() { }

    public virtual void BeginAttack() { }
    public virtual void EndAttack() { }

    public virtual void StartFiring() { }
    public virtual void StopFiring() { }
    public virtual void Fire(Vector2 direction) { }
}
