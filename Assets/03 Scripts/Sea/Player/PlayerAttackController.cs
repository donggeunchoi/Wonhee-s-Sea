using System;
using System.Collections;
using UnityEngine;

public enum WeaponType
{
    Harpoon,
    HarpoonGun,
    SMG
}

public class PlayerAttackController : MonoBehaviour
{
    [Header("무기 오브젝트")]
    [SerializeField] private GameObject _harpoonObject;
    [SerializeField] private GameObject _harpoonGunObject;
    [SerializeField] private GameObject _smgObject;

    [Header("무기 컴포넌트")]
    [SerializeField] private MeleeHarpoonWeapon _harpoonWeapon;
    [SerializeField] private HarpoonGunWeapon _harpoonGunWeapon;
    [SerializeField] private SMGWeapon _smgWeapon;

    [Header("위치 보정")]
    [SerializeField] private Transform _harpoonTransform;
    [SerializeField] private Transform _harpoonGunTransform;
    [SerializeField] private Transform _smgTransform;
    [SerializeField] private float _originY_Right = 0f;
    [SerializeField] private float _originY_Left = 1.1f;

    [Header("근접")]
    [SerializeField] private Transform _harpoonTip;
    [SerializeField, Min(0f)] private float _harpoonAttackDistance = 0.8f;
    [SerializeField] private float _harpoonCooldown = 0.32f;

    private PlayerRoot _owner;
    private bool _isAttacking;
    private bool _isHarpoonAnimating;
    private DungeonPlayerAnimation _animation;
    private Coroutine _harpoonAttack;
    private Vector3 _harpoonTipOrigin;

    private Transform _currentTransform;

    public WeaponType CurrentWeaponType { get; private set; } = WeaponType.Harpoon;


    public void Init(PlayerRoot owner, DungeonPlayerAnimation animation)
    {
        _owner = owner;
        _animation = animation;

        SetWeaponActive(WeaponType.Harpoon, false);
        SetWeaponActive(WeaponType.HarpoonGun, false);
        SetWeaponActive(WeaponType.SMG, false);

        CurrentWeaponType = WeaponType.Harpoon;
        SetWeaponActive(WeaponType.Harpoon, true);

        _currentTransform = _harpoonTransform;

        if (_harpoonWeapon != null)
        {
            _harpoonWeapon.SetOwner(_owner.gameObject);
            _harpoonWeapon.Equip();
        }

        ApplyWeaponSideAndMirror();
    }

    private void SetWeaponActive(WeaponType type, bool active)
    {
        if (_harpoonObject != null && type == WeaponType.Harpoon) _harpoonObject.SetActive(active);
        if (_harpoonGunObject != null && type == WeaponType.HarpoonGun) _harpoonGunObject.SetActive(active);
        if (_smgObject != null && type == WeaponType.SMG) _smgObject.SetActive(active);
    }

    public void SetWeaponType(WeaponType type)
    {
        if (CurrentWeaponType == type) return;

        CancelHarpoonAttack();
        GetCurrentWeapon()?.Unequip();

        SetWeaponActive(CurrentWeaponType, false);
        CurrentWeaponType = type;
        SetWeaponActive(type, true);

        switch (type)
        {
            case WeaponType.Harpoon:
                _currentTransform = _harpoonTransform;
                break;
            case WeaponType.HarpoonGun:
                _currentTransform = _harpoonGunTransform;
                break;
            case WeaponType.SMG:
                _currentTransform = _smgTransform;
                break;
        }

        GetCurrentWeapon()?.SetOwner(_owner.gameObject);
        GetCurrentWeapon()?.Equip();

        ApplyWeaponSideAndMirror();
    }

    private BaseWeapon GetCurrentWeapon()
    {
        return CurrentWeaponType switch
        {
            WeaponType.Harpoon => _harpoonWeapon,
            WeaponType.HarpoonGun => _harpoonGunWeapon,
            WeaponType.SMG => _smgWeapon,
            _ => null
        };
    }

    private void LateUpdate()
    {
        if (_owner == null || _isHarpoonAnimating) return;
        ApplyWeaponSideAndMirror();
    }

    private void ApplyWeaponSideAndMirror()
    {
        if (_currentTransform == null) return;

        bool flip = _animation != null && _animation.IsFlipY;

        Vector3 pos = _currentTransform.localPosition;
        pos.y = flip ? _originY_Left : _originY_Right;
        _currentTransform.localPosition = pos;

        Vector3 scale = _currentTransform.localScale;
        scale.y = flip ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
        _currentTransform.localScale = scale;
    }

    public void OnAttack()
    {
        if (!isActiveAndEnabled || _owner == null || _isAttacking) return;

        Vector2 move = _owner.Input?.ReadMoveInput() ?? Vector2.zero;
        Vector2 facing = move.sqrMagnitude >= PlayerMovement.DeadZone * PlayerMovement.DeadZone
            ? move.normalized
            : (_animation?.FacingDirection ?? Vector2.right);

        if (CurrentWeaponType == WeaponType.Harpoon)
        {
            if (_harpoonTip != null)
                _harpoonAttack = StartCoroutine(C_HarpoonAttack(facing));
            return;
        }

        BaseWeapon weapon = GetCurrentWeapon();
        if (weapon == null || !weapon.CanFire) return;

        _animation?.FaceAttack(facing);
        ApplyWeaponSideAndMirror();
        weapon.Fire(facing);
    }

    private IEnumerator C_HarpoonAttack(Vector2 facing)
    {
        _isAttacking = true;
        _isHarpoonAnimating = true;
        _owner.Movement?.SetAttackLocked(true);

        _animation?.FaceAttack(facing);
        ApplyWeaponSideAndMirror();

        _harpoonTipOrigin = _harpoonTip.localPosition;
        Vector3 origin = _harpoonTipOrigin;
        Vector3 targetPos = origin + Vector3.right * _harpoonAttackDistance;

        _harpoonWeapon?.BeginAttack();

        float elapsed = 0f;
        float attackTime = 0.12f;

        while (elapsed < attackTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / attackTime);
            float eased = Mathf.Sin(t * Mathf.PI * 0.5f);

            _harpoonTip.localPosition = Vector3.Lerp(origin, targetPos, eased);
            yield return null;
        }

        _harpoonWeapon?.EndAttack();

        Vector3 returnStart = _harpoonTip.localPosition;
        elapsed = 0f;
        while (elapsed < 0.10f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.10f;
            _harpoonTip.localPosition = Vector3.Lerp(returnStart, origin, t);
            yield return null;
        }

        _harpoonTip.localPosition = origin;
        _isHarpoonAnimating = false;
        _owner.Movement?.SetAttackLocked(false);

        yield return new WaitForSeconds(_harpoonCooldown);
        _isAttacking = false;
        _harpoonAttack = null;
    }

    private void CancelHarpoonAttack()
    {
        if (_harpoonAttack == null) return;

        StopCoroutine(_harpoonAttack);
        bool restoreOrigin = _isHarpoonAnimating;
        _isHarpoonAnimating = false;
        _owner.Movement?.SetAttackLocked(false);
        _harpoonAttack = null;
        _isAttacking = false;
        _harpoonWeapon?.EndAttack();
        if (restoreOrigin && _harpoonTip != null)
            _harpoonTip.localPosition = _harpoonTipOrigin;
    }

    private void OnDisable()
    {
        CancelHarpoonAttack();
    }
}
