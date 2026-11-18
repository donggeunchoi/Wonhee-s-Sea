using UnityEngine;

public class TownPlayerAnimation : PlayerMovementAnimation
{
    public override void ApplyMovement(Vector2 direction)
    {
        bool moving = direction != Vector2.zero;
        SetMoving(moving);
        if (_anim == null) return;

        int moveDir = 0;
        if (moving)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                moveDir = 1;
                if (_sr != null)
                    _sr.flipX = direction.x < 0f;
            }
            else
            {
                moveDir = direction.y > 0f ? 2 : 3;
            }
        }

        _anim.SetInteger("moveDir", moveDir);
    }
}
