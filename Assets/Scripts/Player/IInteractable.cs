using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerInteract player);
    string GetInteractPrompt();

}
