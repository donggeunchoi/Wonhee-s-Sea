using System;
using System.Collections;
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

    private PlayerRoot _owner;
    private bool _isAttacking;
    private bool _isDaggerAnimating;
    private DungeonPlayerAnimation _animation;
    private Coroutine _daggerAttack;
    private Vector3 _daggerOrigin;

    private Transform _currentTransform;

    public WeaponType CurrentWeaponType { get; private set; } = WeaponType.Dagger;

    public event Action<WeaponType> WeaponChanged;

    public void Init(PlayerRoot owner, DungeonPlayerAnimation animation)
    {
        _owner = owner;
        _animation = animation;

        SetWeaponActive(WeaponType.Dagger, false);
        SetWeaponActive(WeaponType.Harpoon, false);
        SetWeaponActive(WeaponType.SMG, false);

        CurrentWeaponType = WeaponType.Dagger;
        SetWeaponActive(WeaponType.Dagger, true);

        _currentTransform = _daggerTransform;

        if (_daggerWeapon != null)
        {
            _daggerWeapon.SetOwner(_owner.gameObject);
            _daggerWeapon.Equip();
        }

        ApplyWeaponSideAndMirror();
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

        CancelDaggerAttack();
        StopSmgFire();
        GetCurrentWeapon()?.Unequip();

        SetWeaponActive(CurrentWeaponType, false);
        CurrentWeaponType = type;
        SetWeaponActive(type, true);

        switch (type)
        {
            case WeaponType.Dagger:
                _currentTransform = _daggerTransform;
                break;
            case WeaponType.Harpoon:
                _currentTransform = _harpoonTransform;
                break;
            case WeaponType.SMG:
                _currentTransform = _smgTransform;
                break;
        }

        GetCurrentWeapon()?.SetOwner(_owner.gameObject);
        GetCurrentWeapon()?.Equip();

        ApplyWeaponSideAndMirror();
        WeaponChanged?.Invoke(CurrentWeaponType);
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

    private void LateUpdate()
    {
        if (_owner == null || _isDaggerAnimating) return;
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
        if (!isActiveAndEnabled || CurrentWeaponType != WeaponType.Dagger || _isAttacking
            || _owner == null || _daggerTransform == null || _daggerTransform.parent == null) return;

        Vector2 move = _owner.Input?.ReadMoveInput() ?? Vector2.zero;
        Vector2 facing = move.sqrMagnitude >= PlayerMovement.DeadZone * PlayerMovement.DeadZone
            ? move.normalized
            : (_animation?.FacingDirection ?? Vector2.right);

        _daggerAttack = StartCoroutine(C_DaggerAttack(facing));
    }

    private IEnumerator C_DaggerAttack(Vector2 facing)
    {
        _isAttacking = true;
        _isDaggerAnimating = true;
        _owner.Movement?.SetAttackLocked(true);

        _animation?.FaceAttack(facing);
        ApplyWeaponSideAndMirror();

        Vector3 worldDir = new Vector3(facing.x, facing.y, 0f).normalized;
        Vector3 localDir = _daggerTransform.parent.InverseTransformDirection(worldDir);

        _daggerOrigin = _daggerTransform.localPosition;
        Vector3 origin = _daggerOrigin;
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

        Vector3 returnStart = _daggerTransform.localPosition;
        elapsed = 0f;
        while (elapsed < 0.10f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.10f;
            _daggerTransform.localPosition = Vector3.Lerp(returnStart, origin, t);
            yield return null;
        }

        _daggerTransform.localPosition = origin;
        _isDaggerAnimating = false;
        _owner.Movement?.SetAttackLocked(false);

        yield return new WaitForSeconds(_daggerCooldown);
        _isAttacking = false;
        _daggerAttack = null;
    }

    public void FireRanged(Vector2 shootDir)
    {
        if (!isActiveAndEnabled || CurrentWeaponType == WeaponType.Dagger) return;
        if (shootDir.sqrMagnitude < 0.0001f) return;

        if (CurrentWeaponType == WeaponType.SMG)
            _smgWeapon?.StartFiring(shootDir);
        else
            _harpoonWeapon?.Fire(shootDir);
    }

    public void StopSmgFire()
    {
        _smgWeapon?.StopFiring();
    }

    private void CancelDaggerAttack()
    {
        if (_daggerAttack == null) return;

        StopCoroutine(_daggerAttack);
        bool restoreOrigin = _isDaggerAnimating;
        _isDaggerAnimating = false;
        _owner.Movement?.SetAttackLocked(false);
        _daggerAttack = null;
        _isAttacking = false;
        _daggerWeapon?.EndAttack();
        if (restoreOrigin && _daggerTransform != null)
            _daggerTransform.localPosition = _daggerOrigin;
    }

    private void OnDisable()
    {
        StopSmgFire();
        CancelDaggerAttack();
    }
}
