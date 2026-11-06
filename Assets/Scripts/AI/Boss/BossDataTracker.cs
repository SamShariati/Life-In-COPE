using UnityEngine;
using System.Collections.Generic;


public class BossDataTracker : MonoBehaviour
{
    public static BossDataTracker Instance;

    public int nrTrashOnGround = 0;
    public int nrEmptyShelvesLeft = 0;
    public int nrCustomersInLine = 0;
    public int nrTimesCustomersGotHit = 0;
    public int nrTimesBossGotHit = 0;

    private void Awake()
    {
        Instance = this;
    }

}
