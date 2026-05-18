using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;



public class QueueSlot
{
    public GameObject queuePos;
    public CustomerManager occupant;
}


public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance;
    
    public List<GameObject> queuePosList = new List<GameObject>();
    public List<QueueSlot> queue = new List<QueueSlot>();


    private void Awake()
    {
        Instance = this;
        GameObject customerPositions = GameObject.Find("customerPositions");
        foreach (Transform pos in customerPositions.transform)
        {
            queuePosList.Add(pos.gameObject);
        }
        BuildQueue();
    }

    private void BuildQueue()
    {
        foreach (GameObject pos in queuePosList)
        {
            queue.Add(new QueueSlot { queuePos = pos, occupant = null });
        }
    }

    public bool JoinQueue(CustomerManager agent)
    {
        for (int i = 0; i < queue.Count; i++)
        {
            if (queue[i].occupant == null)
            {
                queue[i].occupant = agent;
                agent.assignedSlot = i;
                agent.currentQueuePos = queue[i].queuePos.transform.position;
                //agent.navigation.SetDestination(queue[i].position.transform.position); //ska flyttas in till FSM
                return true;
            }
        }
        return false; // queue is full
    }

    public void AdvanceQueue()
    {
        queue[0].occupant = null;

        for (int i = 1; i < queue.Count; i++)
        {
            if (queue[i].occupant != null)
            {
                CustomerManager customer = queue[i].occupant;
                queue[i].occupant = null;
                queue[i - 1].occupant = customer;
                customer.assignedSlot = i - 1;
                customer.currentQueuePos = queue[i - 1].queuePos.transform.position;
                //customer.navigation.SetDestination(queue[i - 1].position.transform.position); //ska flyttas in till FSM
            }
        }
    }
}
