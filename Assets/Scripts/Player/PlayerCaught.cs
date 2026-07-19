using System.Collections;
using UnityEngine;

public class PlayerCaught
{

    GameObject player;
    private Transform cameraTransform;
    private PlayerMovement playerMovement;
    private CharacterController characterController;

    // Rotation-lock state (used when facing a customer/NPC)
    private Coroutine _rotateCoroutine;
    public bool isFacingTarget = false;

    private const float RotateToTargetDuration = 0.6f;

    public PlayerCaught()
    {
        player = GameObject.FindWithTag("Player");
        cameraTransform = player.transform.Find("Main Camera");
        playerMovement = player.GetComponent<PlayerMovement>();
        characterController = player.GetComponent<CharacterController>();
    }


    public void DisablePlayerMovement()
    {
        playerMovement.enabled = false;
    }

    // -------------------------------------------------------------------------
    // FACE CUSTOMER
    // Freezes player movement and smoothly rotates both the player body (yaw)
    // and the camera (to look directly at faceTarget) towards a customer NPC.
    // -------------------------------------------------------------------------

    public void FaceCustomer(Transform faceTarget)
    {
        if (faceTarget == null) return;

        DisablePlayerMovement();

        if (_rotateCoroutine != null)
        {
            RotateCoroutineRunner.Instance.StopCoroutine(_rotateCoroutine);
        }

        _rotateCoroutine = RotateCoroutineRunner.Instance.StartCoroutine(RotateTowardsTarget(faceTarget));
    }

    // Call this when the interaction ends to hand control back to the player.
    public void ReleaseFromTarget()
    {
        if (_rotateCoroutine != null)
        {
            RotateCoroutineRunner.Instance.StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }

        isFacingTarget = false;
        playerMovement.enabled = true;
    }

    private IEnumerator RotateTowardsTarget(Transform faceTarget)
    {
        isFacingTarget = false;

        // Flat (yaw-only) direction so the player body doesn't tilt up/down.
        Vector3 dirToTargetFlat = faceTarget.position - player.transform.position;
        dirToTargetFlat.y = 0f;

        Quaternion startPlayerRot = player.transform.rotation;
        Quaternion targetPlayerRot = dirToTargetFlat.sqrMagnitude > 0.0001f
            ? Quaternion.LookRotation(dirToTargetFlat.normalized)
            : startPlayerRot;

        Quaternion startCamRot = cameraTransform.localRotation;

        // Camera should look directly at the target (full 3D direction, e.g. the face height).
        Vector3 dirToTarget = (faceTarget.position - cameraTransform.position).normalized;
        Quaternion targetWorldCamRot = Quaternion.LookRotation(dirToTarget, Vector3.up);
        Quaternion targetCamLocalRot = Quaternion.Inverse(targetPlayerRot) * targetWorldCamRot;

        float elapsed = 0f;
        while (elapsed < RotateToTargetDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / RotateToTargetDuration);

            characterController.enabled = false;
            player.transform.rotation = Quaternion.Slerp(startPlayerRot, targetPlayerRot, t);
            characterController.enabled = true;

            cameraTransform.localRotation = Quaternion.Slerp(startCamRot, targetCamLocalRot, t);

            yield return null;
        }

        characterController.enabled = false;
        player.transform.rotation = targetPlayerRot;
        characterController.enabled = true;
        cameraTransform.localRotation = targetCamLocalRot;

        isFacingTarget = true;
        _rotateCoroutine = null;
        Debug.Log(isFacingTarget);
    }
}

// Minimal MonoBehaviour used purely to run coroutines on behalf of PlayerCaught.
// A single shared instance is created the first time it's needed, and reused
// for every PlayerCaught object created afterwards - nothing to create or
// destroy manually per catch event.
public class RotateCoroutineRunner : MonoBehaviour
{
    private static RotateCoroutineRunner _instance;

    public static RotateCoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject runnerGO = new GameObject("RotateCoroutineRunner");
                _instance = runnerGO.AddComponent<RotateCoroutineRunner>();
                GameObject.DontDestroyOnLoad(runnerGO);
            }
            return _instance;
        }
    }
}