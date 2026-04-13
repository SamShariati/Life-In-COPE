using UnityEngine;

public class CardboardBoxObject : MonoBehaviour, IInteractable
{
    public CardboardBoxData data;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initiate(CardboardBoxData _data)
    {
        data = _data;
    }
    public void GetPickedUp(Transform holdPoint)
    {

        rb.isKinematic = true; // disable physics while carried
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void GetDropped()
    {
        rb.isKinematic = false; // re-enable physics
        transform.SetParent(null);
    }

    // IInteractable---------------------------------------
    public string GetInteractPrompt()
    {
        return "Pick up box";
    }

    public void Interact(PlayerInteract player)
    {
        if (!player.Inventory.IsHoldingBox())
        {
            player.Inventory.PickUpBox(this);
            rb.isKinematic = true;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
    //-----------------------------------------------------
}
