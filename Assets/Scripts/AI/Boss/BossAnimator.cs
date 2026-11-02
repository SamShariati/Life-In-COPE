using UnityEngine;




public class BossAnimator : MonoBehaviour
{


    private AnimState _currentState;
    private Animator _animator;

    private static readonly int AnimStateHash = Animator.StringToHash("AnimState");

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetState(AnimState state)
    {
        if (state == _currentState) return;

        _currentState = state;

        
        _animator.SetInteger(AnimStateHash, (int)state);
    }

    public bool IsStateFinished(string tag, int layer = 0)
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(layer);
        return info.IsTag(tag) && info.normalizedTime >= 1f;
    }






}
