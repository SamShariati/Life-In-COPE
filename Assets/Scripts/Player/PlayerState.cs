using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    [HideInInspector] public bool currentlyBeingFollowed;

    private void Awake()
    {
        currentlyBeingFollowed = false;
    }
}
