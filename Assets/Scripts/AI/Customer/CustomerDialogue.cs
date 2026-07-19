using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class CustomerDialogue : MonoBehaviour
{

    public static CustomerDialogue Instance { get; private set; }

    [SerializeField] private GameObject speechBubble;
    [SerializeField] private TextMeshProUGUI text; 

    private void Awake()
    {
    

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        speechBubble.SetActive(false);
    }

    public void ShowBubble()
    {
        speechBubble.SetActive(true);
        text.text = "";
    }

    public void ShowMessage(string msg)
    {

        speechBubble.SetActive(true);
        text.text = msg;

    }

    public void HideMessage()
    {

        speechBubble.SetActive(false);

    }
}
