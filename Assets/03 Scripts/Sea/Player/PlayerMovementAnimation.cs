using UnityEngine;

public abstract class PlayerMovementAnimation : MonoBehaviour
{
    [SerializeField] protected Animator _anim;
    [SerializeField] protected SpriteRenderer _sr;

    public void SetMoving(bool moving)
    {
        if (_anim != null)
            _anim.SetBool("isMove", moving);
    }

    public abstract void ApplyMovement(Vector2 direction);
}
