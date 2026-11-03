using UnityEngine;
using System.Collections.Generic;


public class BossDataTracker : MonoBehaviour
{
    public static BossDataTracker Instance;

    public int nrFoodItemsOnGround = 0;
    public int nrEmptyShelvesLeft = 0;
    public int nrCustomersInLine = 0;

    private void Awake()
    {
        Instance = this;
    }

}
