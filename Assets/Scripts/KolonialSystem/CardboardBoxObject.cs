using TMPro;
using UnityEngine;

public class CardboardBoxObject : MonoBehaviour, IInteractable
{
    public CardboardBoxData data;

    private Rigidbody rb;
    private BoxCollider coll;

    [SerializeField] private TextMeshProUGUI textIDFront;
    [SerializeField] private TextMeshProUGUI textIDBack;
    [SerializeField] private Transform textPosFront;
    [SerializeField] private Transform textPosBack;
    
    //ÄNDRA SÅ ATT ETT PARENTOBJEKT INNEHÅLLER: closedBoxPrefab, openBoxPrefab, OCH ALLA 4 OBJEKT OVAN
    //SKAPA TVÅ METODER: EnableClosedBox, EnableOpenBox. DESSA AKTIVERAS FRÅN SHELF (TROR JAG)
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
        textPosFront = transform.Find("textPosFront");
        textPosBack = transform.Find("textPosBack");
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
    }

    public void Initiate(CardboardBoxData _data)
    {
        data = _data;

        textIDFront.text = data.boxID;
        textIDBack.text = data.boxID;

        textIDFront.transform.position = textPosFront.position;
        textIDBack.transform.position = textPosBack.position;

        textIDFront.transform.rotation = textPosFront.rotation;
        textIDBack.transform.rotation = textPosBack.rotation;

    }
    public void GetPickedUp(Transform holdPoint)
    {

        rb.isKinematic = true; // disable physics while carried
        coll.enabled = false;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void GetDropped()
    {
        rb.isKinematic = false; // re-enable physics
        coll.enabled = true;
        transform.SetParent(null);
    }

    public void GetThrown(float throwForce = 10f)
    {
        rb.isKinematic = false;
        coll.enabled = true;
        transform.SetParent(null);
        Vector3 throwDirection = (transform.forward + transform.up * 0.4f).normalized;
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
    }


    // IInteractable---------------------------------------
    public string GetInteractPrompt(PlayerInteract player)
    {
        if (player.Inventory.IsHoldingBox())
        {
            return "";
        }
        return $"Pick up box ({data.boxID})";

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
