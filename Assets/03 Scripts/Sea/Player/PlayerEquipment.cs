using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public enum WeaponKind
    {
        Melee,
        Ranged
    }

    private PlayerRoot _owner;

    public WeaponKind CurrentWeapon { get; private set; } = WeaponKind.Melee;

    public bool HasHat { get; private set; }
    public bool HasArmor { get; private set; }
    public bool HasShoes { get; private set; }

    public float BonusHp { get; private set; }
    public float BonusMoveSpeed { get; private set; }
    public float BonusOxygen { get; private set; }

    public void Init(PlayerRoot owner)
    {
        _owner = owner;

        EquipMeleeWeapon();
        RecalculateStats();
    }

    public void EquipMeleeWeapon()
    {
        CurrentWeapon = WeaponKind.Melee;
        _owner?.Attack?.SetWeaponType(WeaponType.Melee);
        Debug.Log("무기 장착: 근접");
    }

    public void EquipRangedWeapon()
    {
        CurrentWeapon = WeaponKind.Ranged;
        _owner?.Attack?.SetWeaponType(WeaponType.Ranged);
        Debug.Log("무기 장착: 원거리");
    }

    public void EquipHat()
    {
        HasHat = true;
        RecalculateStats();
        Debug.Log("장비 장착: 모자");
    }

    public void EquipArmor()
    {
        HasArmor = true;
        RecalculateStats();
        Debug.Log("장비 장착: 갑옷");
    }

    public void EquipShoes()
    {
        HasShoes = true;
        RecalculateStats();
        Debug.Log("장비 장착: 신발");
    }

    public void UnequipHat()
    {
        HasHat = false;
        RecalculateStats();
        Debug.Log("장비 해제: 모자");
    }

    public void UnequipArmor()
    {
        HasArmor = false;
        RecalculateStats();
        Debug.Log("장비 해제: 갑옷");
    }

    public void UnequipShoes()
    {
        HasShoes = false;
        RecalculateStats();
        Debug.Log("장비 해제: 신발");
    }

    private void RecalculateStats()
    {
        BonusHp = 0f;
        BonusMoveSpeed = 0f;
        BonusOxygen = 0f;

        // 임시 수치
        if (HasHat)
            BonusOxygen += 20f;

        if (HasArmor)
            BonusHp += 50f;

        if (HasShoes)
            BonusMoveSpeed += 1f;

        _owner?.Stats?.RefreshByEquipment();
        _owner?.Oxygen?.RefreshByEquipment();
    }
}
