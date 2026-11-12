using TMPro;
using UnityEngine;

public class BossSpeechBubble : MonoBehaviour
{

    private TextMeshProUGUI label;
    private Transform root;
    private Transform player;
    private Camera cam;
    void Awake()
    {
        player = GameObject.Find("Player").transform;

        root = transform.Find("speechAnchor");
        Transform labelObject = transform.Find("speechAnchor/Canvas/speechLabel");
        label = labelObject.GetComponent<TextMeshProUGUI>();

        cam = Camera.main;
    }
    void LateUpdate()
    {

        Vector3 dir = cam.transform.position - root.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        root.rotation = Quaternion.LookRotation(dir);
    }


}
