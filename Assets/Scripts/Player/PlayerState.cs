using System.Collections;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    public bool currentlyBeingFollowed;
    public bool inStockingMode;
    public bool inScanningMode;

    public PlayerCaught _activeCaught;


    private void Awake()
    {
        Instance = this;
        currentlyBeingFollowed = false;
        inStockingMode = false;
        inScanningMode = false;
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

