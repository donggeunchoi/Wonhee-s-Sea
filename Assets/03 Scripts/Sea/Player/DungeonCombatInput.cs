using UnityEngine;

public class DungeonCombatInput : MonoBehaviour
{
    [SerializeField] private FloatingJoystick _aimJoystick;
    [SerializeField] private GameObject _attackButtonObj;
    [SerializeField] private GameObject _aimJoystickObj;

    private PlayerRoot _owner;
    private DungeonPlayerAnimation _animation;
    private bool _isAiming;
    private bool _subscribed;

    public void Init(PlayerRoot owner, DungeonPlayerAnimation animation)
    {
        _owner = owner;
        _animation = animation;
        if (isActiveAndEnabled)
            Subscribe();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        if (_subscribed || _owner == null || _owner.Attack == null) return;

        _subscribed = true;
        _owner.Attack.WeaponChanged += RefreshWeaponControls;
        if (_aimJoystick != null)
        {
            _aimJoystick.OnReleased += HandleAimReleased;
            _aimJoystick.OnCanceled += StopAiming;
        }

        RefreshWeaponControls(_owner.Attack.CurrentWeaponType);
    }

    private void OnDisable()
    {
        if (!_subscribed) return;

        _subscribed = false;
        _owner.Attack.WeaponChanged -= RefreshWeaponControls;
        if (_aimJoystick != null)
        {
            _aimJoystick.OnReleased -= HandleAimReleased;
            _aimJoystick.OnCanceled -= StopAiming;
            _aimJoystick.CancelInput();
        }

        StopAiming();
    }

    private void Update()
    {
        if (_owner == null || _owner.Attack == null) return;

        if (_owner.Attack.CurrentWeaponType == WeaponType.Dagger)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                OnAttackPressed();

            return;
        }

        if (_aimJoystick == null || !_aimJoystick.HasDirection)
        {
            if (_isAiming)
                StopAiming();

            return;
        }

        _isAiming = true;
        _owner.Movement?.SetMovementSuppressed(true);

        // 조이스틱을 당기는 방향의 반대로 조준한다.
        Vector2 aimDirection = -_aimJoystick.Direction.normalized;
        _animation?.FaceAim(aimDirection);

        if (_owner.Attack.CurrentWeaponType == WeaponType.SMG)
            _owner.Attack.FireRanged(aimDirection);
    }

    public void OnAttackPressed()
    {
        if (isActiveAndEnabled && _owner != null)
            _owner.Attack?.OnAttack();
    }

    private void HandleAimReleased(Vector2 direction)
    {
        StopAiming();

        if (_owner.Attack.CurrentWeaponType != WeaponType.Harpoon || direction == Vector2.zero)
            return;

        Vector2 aimDirection = -direction;
        _animation?.FaceAim(aimDirection);
        _owner.Attack.FireRanged(aimDirection);
    }

    private void StopAiming()
    {
        _isAiming = false;
        _owner.Movement?.SetMovementSuppressed(false);
        _owner.Attack.StopSmgFire();
    }

    private void RefreshWeaponControls(WeaponType weapon)
    {
        _aimJoystick?.CancelInput();
        StopAiming();

        bool isMelee = weapon == WeaponType.Dagger;
        if (_attackButtonObj != null) _attackButtonObj.SetActive(isMelee);
        if (_aimJoystickObj != null) _aimJoystickObj.SetActive(!isMelee);
    }
}
