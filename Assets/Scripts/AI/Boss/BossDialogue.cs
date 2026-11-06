using System.Collections.Generic;
using UnityEngine;

public class BossDialogue : MonoBehaviour
{
    
    public static BossDialogue instance;

    void Awake()
    {
        instance = this;
    }


    public string GetComplainDialogue(string type)
    {
        string message = "";
        if (type == "trash")
        {
            message = $"I've spotted some trash near the foodstalls, pick that up! I've counted {BossDataTracker.Instance.nrTrashOnGround} items so far.";
        }
        else if (type == "shelf")
        {
            message = $"You're slacking with the shelves and are behind schedule, pick up the pace! I've counted {BossDataTracker.Instance.nrEmptyShelvesLeft} left" +
                $"so far.";
        }
        else if (type == "hitBoss")
        {
            message = $"I do not appreciate being by stuff, neither by you or anyone else! You do not want to piss me off, understood?";
        }
        else if (type == "hitCustomers")
        {
            message = $"I've received complains from our customers that you treat them with badly. Totally unacceptable, be more professional!";
        }
        else if (type == "register")
        {
            message = $"I do not appreciate that our customers have to wait long in line. {BossDataTracker.Instance.nrCustomersInLine} is already too many in line!";
        }
        return message;

        
    }
    
}
