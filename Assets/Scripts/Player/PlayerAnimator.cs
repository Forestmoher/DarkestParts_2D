using UnityEngine;

public enum PlayerFacing { up, down, side }

public class PlayerAnimator : ActorAnimator
{

    private string _walkUpAnim = "Walk_Up";
    private string _walkDownAnim = "Walk_Down";
    private string _walkSideAnim = "Walk_Side";
    private string _idleDownAnim = "Idle_Down";
    private string _idleUpAnim = "Idle_Up";
    private string _idleSideAnim = "Idle_Side";
    private PlayerFacing _playerFacing;

    private string _currentAnimation;

    private void Start()
    {
        _currentAnimation = _idleDownAnim;
    }

    public override void SetAnimation(Vector2 input)
    {
        if(input.magnitude == 0)
        {
            if (_playerFacing == PlayerFacing.up) _animator.Play(_idleUpAnim); _currentAnimation = _idleUpAnim;
            if (_playerFacing == PlayerFacing.down) _animator.Play(_idleDownAnim); _currentAnimation = _idleDownAnim;
            if (_playerFacing == PlayerFacing.side) _animator.Play(_idleSideAnim); _currentAnimation = _idleSideAnim;
        }
        else
        {
            if(input.y > .01)
            {
                _playerFacing = PlayerFacing.up;
                if (_currentAnimation == _walkUpAnim) return;
                _currentAnimation = _walkUpAnim;
            }
            else if(input.y < -.01)
            {
                _playerFacing = PlayerFacing.down;
                if (_currentAnimation == _walkDownAnim) return;
                _currentAnimation = _walkDownAnim;
            }
            else if (input.x > .01)
            {
                _playerFacing = PlayerFacing.side;
                _sprite.flipX = true;
                if(_currentAnimation == _walkSideAnim) return;
                _currentAnimation = _walkSideAnim;
            }
            else if (input.x < -.01)
            {
                _playerFacing = PlayerFacing.side;
                _sprite.flipX = false;
                if (_currentAnimation == _walkSideAnim) return;
                _currentAnimation = _walkSideAnim;
            }
            _animator.Play(_currentAnimation);
        }
    }
}
