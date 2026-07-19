using System.Collections;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    [HideInInspector] public bool currentlyBeingFollowed;

    public PlayerCaught _activeCaught;


    private void Awake()
    {
        Instance = this;
        currentlyBeingFollowed = false;
    }

    public void CaughtPlayer(Transform customerHead)
    {
        _activeCaught = new PlayerCaught();
        _activeCaught.FaceCustomer(customerHead);
    }

    public void ReleasePlayer()
    {
        if (_activeCaught == null) return;

        _activeCaught.ReleaseFromTarget();
        _activeCaught = null;
    }


}

