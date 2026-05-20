using UnityEngine;

public class CashRegister : MonoBehaviour, IInteractable
{
    public string GetInteractPrompt(PlayerInteract player)
    {
        //float distance = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(distance);

        return "Test";

    }

    public void Interact(PlayerInteract player)
    {
      

    }
}
