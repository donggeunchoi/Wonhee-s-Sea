using UnityEngine;

public class DungeonPlayerAnimation : PlayerMovementAnimation
{
    [SerializeField] private Transform _pivot;
    [SerializeField] private Transform _weaponGrip;
    [SerializeField] private Sprite _secondSwimFrame;
    [SerializeField] private Vector2 _firstSwimGripOffset;
    [SerializeField] private Vector2 _secondSwimGripOffset;

    private const float MaxTurnSpeed = 360f;
    private const float MovementFlipDeadZone = 10f;
    private const float PivotYRight = 0f;
    private const float PivotYLeft = -0.6f;

    public Vector2 FacingDirection { get; private set; } = Vector2.right;
    public bool IsFlipY { get; private set; }

    private Vector3 _weaponGripOrigin;

    private void Awake()
    {
        if (_weaponGrip != null)
            _weaponGripOrigin = _weaponGrip.localPosition;
    }

    private void LateUpdate()
    {
        UpdateWeaponGrip();
    }

    private void UpdateWeaponGrip()
    {
        if (_weaponGrip == null || _sr == null) return;

        Vector2 offset = _sr.sprite == _secondSwimFrame ? _secondSwimGripOffset : _firstSwimGripOffset;
        if (IsFlipY)
            offset.y = -offset.y;

        _weaponGrip.localPosition = _weaponGripOrigin + (Vector3)offset;
    }

    public void FaceAim(Vector2 direction)
    {
        FacingDirection = direction;
    }

    public void FaceAttack(Vector2 direction)
    {
        FacingDirection = direction;
        ApplyFacing(snap: true);
        UpdateWeaponGrip();
    }

    public override void ApplyMovement(Vector2 direction)
    {
        bool moving = direction != Vector2.zero;
        SetMoving(moving);

        if (moving)
            FacingDirection = direction.normalized;

        ApplyFacing(snap: false);
    }

    private void ApplyFacing(bool snap)
    {
        if (_pivot == null) return;

        float angle = Mathf.Atan2(FacingDirection.y, FacingDirection.x) * Mathf.Rad2Deg;
        float nextAngle = snap ? angle : Mathf.MoveTowardsAngle(
            _pivot.localEulerAngles.z, angle, MaxTurnSpeed * Time.fixedDeltaTime);
        _pivot.localRotation = Quaternion.Euler(0f, 0f, nextAngle);

        if (_sr == null) return;

        float absoluteAngle = Mathf.Abs(Mathf.DeltaAngle(0f, nextAngle));
        if (!snap && Mathf.Abs(absoluteAngle - 90f) <= MovementFlipDeadZone) return;

        bool flip = absoluteAngle > 90f;
        if (flip == IsFlipY) return;

        IsFlipY = flip;
        _sr.flipY = flip;
        Vector3 position = _pivot.localPosition;
        position.y = IsFlipY ? PivotYLeft : PivotYRight;
        _pivot.localPosition = position;
    }
}
