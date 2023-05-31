using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class ActorAnimator : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    protected SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    public virtual void SetAnimation(Vector2 input)
    {

    }
}