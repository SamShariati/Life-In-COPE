using UnityEngine;

public enum AnimState
{
    Idle = 0,
    Walk = 1,
    Chase = 2,
    CaughtPlayer = 3,
    GrabItem = 4,
    FallBackward = 5,
    GetUpForward = 6,
    FallForward = 7,
    GetUpBackward = 8,
    Dizzy = 9,
    Thank = 10

}

public class CustomerAnimator : MonoBehaviour
{
    private AnimState _currentState;
    private Animator _animator;

    private static readonly int AnimStateHash = Animator.StringToHash("AnimState");
    private static readonly int IdleVariantHash = Animator.StringToHash("IdleVariant");
    private static readonly int DizzyVariantHash = Animator.StringToHash("DizzyVariant");
    private static readonly int ThankVariantHash = Animator.StringToHash("ThankVariant");


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
        else if (state == AnimState.Dizzy)
        {
            SetDizzy();
            return;
        }
        else if (state == AnimState.Thank)
        {
            SetThank();
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

    private void SetDizzy()
    {
        float randomIdle = Random.Range(0, 2);

        _animator.SetInteger(AnimStateHash, (int)AnimState.Dizzy);
        _animator.SetFloat(DizzyVariantHash, randomIdle);
    }

    private void SetThank()
    {
        float randomIdle = Random.Range(0, 11);

        _animator.SetInteger(AnimStateHash, (int)AnimState.Thank);
        _animator.SetFloat(ThankVariantHash, randomIdle);
    }

    public bool IsStateFinished(string tag, int layer = 0)
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(layer);
        return info.IsTag(tag) && info.normalizedTime >= 1f;
    }

}