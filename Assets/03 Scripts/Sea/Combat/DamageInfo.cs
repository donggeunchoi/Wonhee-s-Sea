using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageInfo
{
    public GameObject Attacker;
    public float Damage;
    public Vector2 HitPoint;
    public Vector2 HitNormal;

    public DamageInfo(GameObject attacker, float damage, Vector2 hitPoint, Vector2 hitNormal)
    {
        Attacker = attacker;
        Damage = damage;
        HitPoint = hitPoint;
        HitNormal = hitNormal;
    }
}
