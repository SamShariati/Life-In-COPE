using TMPro;
using UnityEngine;

public class BossSpeechBubble : MonoBehaviour
{
    [SerializeField] private float charsPerSecond = 30f;

    private TextMeshProUGUI label;
    private Transform root;
    private Transform player;
    private Camera cam;

    private float idleTimer = 0f;
    private bool speechEnabled = false;

    // Typewriter state
    private bool typing = false;
    private float visibleChars = 0f;
    private int totalChars = 0;
    private float holdDuration = 0f;

    void Awake()
    {
        player = GameObject.Find("Player").transform;

        root = transform.Find("speechAnchor");
        Transform labelObject = transform.Find("speechAnchor/Canvas/speechLabel");
        label = labelObject.GetComponent<TextMeshProUGUI>();

        cam = Camera.main;
        root.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!speechEnabled) return;

        if (typing)
        {
            TypeText(Time.deltaTime);
        }
        else
        {
            SpeechDuration();
        }
    }

    void LateUpdate()
    {
        if (speechEnabled)
        {
            Vector3 dir = cam.transform.position - root.position;
            if (dir.sqrMagnitude < 0.0001f) return;
            root.rotation = Quaternion.LookRotation(dir);
        }
    }

    public void Speak(string speech, float duration)
    {
        speechEnabled = true;
        root.gameObject.SetActive(true);

        label.text = speech;
        label.maxVisibleCharacters = 0;
        label.ForceMeshUpdate(); // makes characterCount accurate right away
        totalChars = label.textInfo.characterCount;

        visibleChars = 0f;
        holdDuration = duration;
        typing = true;
    }

    private void TypeText(float delta)
    {
        visibleChars += charsPerSecond * delta;
        int count = Mathf.Min(Mathf.FloorToInt(visibleChars), totalChars);
        label.maxVisibleCharacters = count;

        if (count >= totalChars)
        {
            typing = false;
            SetTimer(holdDuration); // start the "stay on screen" countdown
        }
    }

    private bool TickTimer(float delta)
    {
        idleTimer -= delta;
        return idleTimer <= 0;
    }

    private void SetTimer(float duration)
    {
        idleTimer = duration;
    }

    private void SpeechDuration()
    {
        if (TickTimer(Time.deltaTime))
        {
            root.gameObject.SetActive(false);
            speechEnabled = false;
        }
    }
}