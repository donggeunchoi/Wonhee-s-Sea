using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public enum WeaponKind
    {
        Harpoon,
        HarpoonGun,
        SMG
    }

    private PlayerRoot _owner;

    const int UNDERWATER_KNIFE_ID = 50001;
    const int HARPOON_ID = 50002;
    const int HARPOON_GUN_ID = 50003;
    const int UNDERWATER_GUN_ID = 50004;

    public int EquippedWeaponID { get; private set; }
    public WeaponKind CurrentWeapon { get; private set; } = WeaponKind.Harpoon;

    public bool HasHat { get; private set; }
    public bool HasArmor { get; private set; }
    public bool HasShoes { get; private set; }

    public float BonusHp { get; private set; }
    public float BonusMoveSpeed { get; private set; }
    public float BonusOxygen { get; private set; }

    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        var equipInfo = UserManager.Ins?.equipInfo;
        EquippedWeaponID = equipInfo != null && equipInfo.TryGetWeaponLevel( equipInfo.EquipWeapon, out _ ) ? equipInfo.EquipWeapon : 0;

        switch( EquippedWeaponID )
        {
            case HARPOON_GUN_ID:
                EquipHarpoonGun();
                break;
            case UNDERWATER_GUN_ID:
                EquipSMG();
                break;
            case UNDERWATER_KNIFE_ID:
            case HARPOON_ID:
            default:
                EquipHarpoon();
                break;
        }

        RecalculateStats();
    }

    public void EquipHarpoon()
    {
        CurrentWeapon = WeaponKind.Harpoon;
        _owner?.Attack?.SetWeaponType(WeaponType.Harpoon);
        Debug.Log("무기 장착: 작살");
    }

    public void EquipHarpoonGun()
    {
        CurrentWeapon = WeaponKind.HarpoonGun;
        _owner?.Attack?.SetWeaponType(WeaponType.HarpoonGun);
        Debug.Log("무기 장착: 작살총");
    }

    public void EquipSMG()
    {
        CurrentWeapon = WeaponKind.SMG;
        _owner?.Attack?.SetWeaponType(WeaponType.SMG);
        Debug.Log("무기 장착: 따발총");
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
