using UnityEngine;

public class DungeonPlayerAnimation : PlayerMovementAnimation
{
    [SerializeField] private Transform _pivot;

    private const float MaxTurnSpeed = 360f;
    private const float PivotYRight = 0f;
    private const float PivotYLeft = -0.6f;

    public Vector2 FacingDirection { get; private set; } = Vector2.right;
    public bool IsFlipY { get; private set; }

    public void FaceAim(Vector2 direction)
    {
        FacingDirection = direction;
    }

    public void FaceAttack(Vector2 direction)
    {
        FacingDirection = direction;
        ApplyFacing(snap: true);
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

        bool flip = angle > 90f || angle < -90f;
        if (flip == IsFlipY) return;

        IsFlipY = flip;
        _sr.flipY = flip;
        Vector3 position = _pivot.localPosition;
        position.y = IsFlipY ? PivotYLeft : PivotYRight;
        _pivot.localPosition = position;
    }
}
