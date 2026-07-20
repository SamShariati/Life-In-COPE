using UnityEngine;

public enum AnimState
{
    Idle = 0,
    Walk = 1,
    Chase = 2,
    CaughtPlayer = 3,
    Hurt = 4,
    Death = 5
}

public class CustomerAnimator : MonoBehaviour
{
    private AnimState _currentState;
    private Animator _animator;

    private static readonly int AnimStateHash = Animator.StringToHash("AnimState");
    private static readonly int IdleVariantHash = Animator.StringToHash("IdleVariant");
    

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetState(AnimState state)
    {
        if (state == _currentState) return;

        _currentState = state;

        if (state == AnimState.Idle)
        {
            SetIdle();
            return; 
        }

        _animator.SetInteger(AnimStateHash, (int)state);
    }

    private void SetIdle()
    {
        float randomIdle = Random.Range(0, 5);

        _animator.SetInteger(AnimStateHash, (int)AnimState.Idle);
        _animator.SetFloat(IdleVariantHash, randomIdle);
    }
}