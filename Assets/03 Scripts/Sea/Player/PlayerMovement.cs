using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 3f;

    private const float Acceleration = 10f;
    internal const float DeadZone = 0.15f;

    private PlayerRoot _owner;
    private Rigidbody2D _body;
    private PlayerMovementAnimation _animation;
    private float _freezeUntil;
    private bool _movementSuppressed;

    public void Init(PlayerRoot owner)
    {
        _owner = owner;
        _body = owner.Rb;
        _animation = GetComponent<PlayerMovementAnimation>();
        if (_body == null) return;

        _body.gravityScale = 0f;
        _body.drag = 3f;
        _body.angularDrag = 3f;
        _body.interpolation = RigidbodyInterpolation2D.Interpolate;
        _body.freezeRotation = true;
    }

    public void SetMovementSuppressed(bool suppressed)
    {
        _movementSuppressed = suppressed;
    }

    public void Freeze(float duration)
    {
        _freezeUntil = Mathf.Max(_freezeUntil, Time.time + duration);
        if (_body != null)
            _body.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (_owner == null || _owner.Input == null || _body == null) return;

        if (Time.time < _freezeUntil)
        {
            _body.velocity = Vector2.zero;
            _animation?.SetMoving(false);
            return;
        }

        Vector2 direction = _movementSuppressed ? Vector2.zero : _owner.Input.Move;
        if (direction.sqrMagnitude < DeadZone * DeadZone)
            direction = Vector2.zero;

        float speed = _maxSpeed + (_owner.Equipment?.BonusMoveSpeed ?? 0f);
        _body.velocity = Vector2.Lerp(
            _body.velocity, direction * speed, Acceleration * Time.fixedDeltaTime);

        _animation?.ApplyMovement(direction);
    }

    private void OnDisable()
    {
        _freezeUntil = 0f;
        _movementSuppressed = false;

        if (_body != null)
            _body.velocity = Vector2.zero;

        _animation?.SetMoving(false);
    }
}
