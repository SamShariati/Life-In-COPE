using UnityEngine;

public class InspectionConditions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RollConfusedType(CustomerManager agent)
    {
        float randPercent = Random.Range(0, 100);

        if (randPercent < 66.7f)
        {
            agent.BB.Confused.isCurrentlyPatrolling = true;
        }
        else
        {
            agent.BB.Confused.isCurrCheckingWrongShelf = true;
        }

    }
}
