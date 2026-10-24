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
    Thank = 10,
    LookAround = 11,
    Attack,
    Pleased,
    Taunt

}

public class CustomerAnimator : MonoBehaviour
{
    private AnimState _currentState;
    private Animator _animator;

    private static readonly int AnimStateHash = Animator.StringToHash("AnimState");
    private static readonly int IdleVariantHash = Animator.StringToHash("IdleVariant");
    private static readonly int DizzyVariantHash = Animator.StringToHash("DizzyVariant");
    private static readonly int ThankVariantHash = Animator.StringToHash("ThankVariant");
    private static readonly int LookAroundVariantHash = Animator.StringToHash("LookAroundVariant");
    private static readonly int AttackVariantHash = Animator.StringToHash("AttackVariant");
    private static readonly int PleasedVariantHash = Animator.StringToHash("PleasedVariant");
    private static readonly int TauntVariantHash = Animator.StringToHash("TauntVariant");


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
        else if (state == AnimState.LookAround)
        {
            SetLookAround();
            return;
        }
        else if (state == AnimState.Attack)
        {
            SetAttack();
            return;
        }
        else if (state == AnimState.Pleased)
        {
            SetPleased();
            return;
        }
        else if (state == AnimState.Taunt)
        {
            SetTaunt();
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

    private void SetLookAround()
    {
        float randomIdle = Random.Range(0, 3);

        _animator.SetInteger(AnimStateHash, (int)AnimState.LookAround);
        _animator.SetFloat(LookAroundVariantHash, randomIdle);
    }

    private void SetAttack()
    {
        float randomIdle = Random.Range(0, 4);

        _animator.SetInteger(AnimStateHash, (int)AnimState.Attack);
        _animator.SetFloat(AttackVariantHash, randomIdle);
    }

    private void SetPleased()
    {
        float randomIdle = Random.Range(0, 3);

        _animator.SetInteger(AnimStateHash, (int)AnimState.Pleased);
        _animator.SetFloat(PleasedVariantHash, randomIdle);
    }

    private void SetTaunt()
    {
        float randomIdle = Random.Range(0, 4);

        _animator.SetInteger(AnimStateHash, (int)AnimState.Taunt);
        _animator.SetFloat(TauntVariantHash, randomIdle);
    }



    public bool IsStateFinished(string tag, int layer = 0)
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(layer);
        return info.IsTag(tag) && info.normalizedTime >= 1f;
    }

}