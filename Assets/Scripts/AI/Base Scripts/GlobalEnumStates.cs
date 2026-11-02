using UnityEngine;

public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE
}

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
    Attack = 12,
    Pleased = 13,
    Taunt = 14,
    Inspect = 15,
    Dissappoint = 16

}
