using System.Runtime.CompilerServices;
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

    public bool IsInFlight { get; private set; }
    public Vector3 ThrownDirection { get; private set; }

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
        ClearThrow();
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

        Vector3 flat = new Vector3(throwDirection.x, 0f, throwDirection.z);
        ThrownDirection = flat.normalized;
        IsInFlight = true;
    }


    public void ClearThrow()
    {
        IsInFlight = false;
        ThrownDirection = Vector3.zero;
    }


    // also call ClearThrow() at the top of GetPickedUp()

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsInFlight) return;

        // let the customer consume the throw, and ignore the thrower
        if (collision.gameObject.CompareTag("Customer AI") ||
            collision.gameObject.CompareTag("Player")) return;

        ClearThrow(); // ground, wall, shelf, another box - no longer a valid projectile
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
