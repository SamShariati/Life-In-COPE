using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMBaseState
{

    public abstract void EnterState(CustomerManager agent);

    public abstract void UpdateState(CustomerManager agent);


    
}
