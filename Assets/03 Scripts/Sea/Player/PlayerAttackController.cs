using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Dagger,
    Harpoon,
    SMG
}

public class PlayerAttackController : MonoBehaviour
{
    [Header("무기 오브젝트")]
    [SerializeField] private GameObject _daggerObject;
    [SerializeField] private GameObject _harpoonObject;
    [SerializeField] private GameObject _smgObject;

    [Header("무기 컴포넌트")]
    [SerializeField] private DaggerWeapon _daggerWeapon;
    [SerializeField] private HarpoonWeapon _harpoonWeapon;
    [SerializeField] private SMGWeapon _smgWeapon;

    [Header("위치 보정")]
    [SerializeField] private Transform _daggerTransform;
    [SerializeField] private Transform _harpoonTransform;
    [SerializeField] private Transform _smgTransform;
    [SerializeField] private float _originY_Right = 0f;
    [SerializeField] private float _originY_Left = 1.1f;

    [Header("근접")]
    [SerializeField] private float _daggerAttackDistance = 1.1f;
    [SerializeField] private float _daggerCooldown = 0.32f;

    [Header("원거리")]
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private float _projectileSpeed = 16f;

    [Header("공격 버튼")]
    [SerializeField] private GameObject _attackButtonObj;
    [SerializeField] private GameObject _aimJoystickObj;

    private PlayerRoot _owner;
    private bool _isAttacking;
    private float _lastFacingX = 1f;

    private Transform _currentTransform;
    private Transform _currentModel;

    public WeaponType CurrentWeaponType { get; private set; } = WeaponType.Dagger;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;

        SetWeaponActive(WeaponType.Dagger, false);
        SetWeaponActive(WeaponType.Harpoon, false);
        SetWeaponActive(WeaponType.SMG, false);

        CurrentWeaponType = WeaponType.Dagger;
        SetWeaponActive(WeaponType.Dagger, true);

        _currentTransform = _daggerTransform;
        _currentModel = _daggerTransform;

        if (_daggerWeapon != null)
        {
            _daggerWeapon.SetOwner(_owner.gameObject);
            _daggerWeapon.Equip();
        }

        ApplyModeUI();
        ApplyWeaponSideAndMirror(true);
    }

    private void SetWeaponActive(WeaponType type, bool active)
    {
        if (_daggerObject != null && type == WeaponType.Dagger) _daggerObject.SetActive(active);
        if (_harpoonObject != null && type == WeaponType.Harpoon) _harpoonObject.SetActive(active);
        if (_smgObject != null && type == WeaponType.SMG) _smgObject.SetActive(active);
    }

    public void SetWeaponType(WeaponType type)
    {
        if (CurrentWeaponType == type) return;

        GetCurrentWeapon()?.Unequip();

        SetWeaponActive(CurrentWeaponType, false);
        CurrentWeaponType = type;
        SetWeaponActive(type, true);

        switch (type)
        {
            case WeaponType.Dagger:
                _currentTransform = _daggerTransform;
                _currentModel = _daggerTransform;
                break;
            case WeaponType.Harpoon:
                _currentTransform = _harpoonTransform;
                _currentModel = _harpoonTransform;
                break;
            case WeaponType.SMG:
                _currentTransform = _smgTransform;
                _currentModel = _smgTransform;
                break;
        }

        GetCurrentWeapon()?.SetOwner(_owner.gameObject);
        GetCurrentWeapon()?.Equip();

        ApplyModeUI();
        ApplyWeaponSideAndMirror(true);
    }

    private BaseWeapon GetCurrentWeapon()
    {
        return CurrentWeaponType switch
        {
            WeaponType.Dagger => _daggerWeapon,
            WeaponType.Harpoon => _harpoonWeapon,
            WeaponType.SMG => _smgWeapon,
            _ => null
        };
    }

    private void ApplyModeUI()
    {
        bool isMelee = CurrentWeaponType == WeaponType.Dagger;
        if (_attackButtonObj != null) _attackButtonObj.SetActive(isMelee);
        if (_aimJoystickObj != null) _aimJoystickObj.SetActive(!isMelee);
    }

    private void LateUpdate()
    {
        if (_owner == null || _isAttacking) return;
        ApplyWeaponSideAndMirror(false);
    }

    private void ApplyWeaponSideAndMirror(bool forceSnap)
    {
        if (_currentTransform == null) return;

        Vector2 facing = _owner?.Input?.FacingDir ?? Vector2.right;
        if (Mathf.Abs(facing.x) > 0.01f) _lastFacingX = Mathf.Sign(facing.x);

        bool flip = _owner?.Controller?.IsFlipY ?? (_lastFacingX < 0f);

        Vector3 pos = _currentTransform.localPosition;
        pos.y = flip ? _originY_Left : _originY_Right;
        _currentTransform.localPosition = pos;

        if (_currentModel != null)
        {
            Vector3 s = _currentModel.localScale;
            s.y = flip ? -Mathf.Abs(s.y) : Mathf.Abs(s.y);
            _currentModel.localScale = s;
        }
    }

    public void OnAttack()
    {
        if (CurrentWeaponType != WeaponType.Dagger || _isAttacking) return;
        StartCoroutine(C_DaggerAttack());
    }

    private IEnumerator C_DaggerAttack()
    {
        _isAttacking = true;
        _owner.Controller?.Freeze(0.5f);

        ApplyWeaponSideAndMirror(forceSnap: true);

        Vector2 facing = _owner.Input?.FacingDir ?? Vector2.right;

        Vector3 worldDir = new Vector3(facing.x, facing.y, 0f).normalized;
        Vector3 localDir = _daggerTransform.parent.InverseTransformDirection(worldDir);

        Vector3 origin = _daggerTransform.localPosition;
        Vector3 targetPos = origin + localDir * _daggerAttackDistance;

        _daggerWeapon?.BeginAttack();

        float elapsed = 0f;
        float attackTime = 0.12f;

        while (elapsed < attackTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / attackTime;
            float eased = Mathf.Sin(t * Mathf.PI * 0.8f);

            _daggerTransform.localPosition = Vector3.Lerp(origin, targetPos, eased);
            yield return null;
        }

        _daggerWeapon?.EndAttack();

        elapsed = 0f;
        while (elapsed < 0.10f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.10f;
            _daggerTransform.localPosition = Vector3.Lerp(targetPos, origin, t);
            yield return null;
        }

        _daggerTransform.localPosition = origin;

        yield return new WaitForSeconds(_daggerCooldown);
        _isAttacking = false;
    }

    public void FireRanged(Vector2 shootDir)
    {
        if (CurrentWeaponType == WeaponType.Dagger) return;
        if (shootDir.sqrMagnitude < 0.0001f) return;

        if (CurrentWeaponType == WeaponType.SMG)
            _smgWeapon?.StartFiring();
        else
            _harpoonWeapon?.Fire(shootDir);
    }

    public void StopSmgFire()
    {
        _smgWeapon?.StopFiring();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CurrentWeaponType == WeaponType.Dagger)
            OnAttack();
    }
}
