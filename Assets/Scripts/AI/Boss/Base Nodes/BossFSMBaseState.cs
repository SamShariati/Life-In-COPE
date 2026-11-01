using UnityEngine;

public abstract class BossFSMBaseState
{
    public abstract void EnterState(BossManager agent);

    public abstract void UpdateState(BossManager agent);
}
