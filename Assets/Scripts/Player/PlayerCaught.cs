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
    private const float HeadTrackSmoothSpeed = 8f; // how snappy the camera follows head movement once locked on

    private Transform _currentFaceTarget;

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
        _currentFaceTarget = null;
        playerMovement.enabled = true;
    }

    private IEnumerator RotateTowardsTarget(Transform faceTarget)
    {
        isFacingTarget = false;
        _currentFaceTarget = faceTarget;

        // Flat (yaw-only) direction so the player body doesn't tilt up/down.
        // The player's BODY only turns once, toward the target's starting position -
        // it's the camera that keeps following afterward (e.g. a dancing customer's head).
        Vector3 dirToTargetFlat = faceTarget.position - player.transform.position;
        dirToTargetFlat.y = 0f;

        Quaternion startPlayerRot = player.transform.rotation;
        Quaternion targetPlayerRot = dirToTargetFlat.sqrMagnitude > 0.0001f
            ? Quaternion.LookRotation(dirToTargetFlat.normalized)
            : startPlayerRot;

        Quaternion startCamRot = cameraTransform.localRotation;

        // -------------------------------------------------------------------
        // PHASE 1: Ease-in. Player body turns to face the target's general
        // direction over RotateToTargetDuration. The camera target is
        // recalculated every frame from the target's CURRENT position (not
        // a one-time snapshot), so if the target is already moving/dancing
        // during the ease-in, the camera blends toward wherever it currently is.
        // -------------------------------------------------------------------
        float elapsed = 0f;
        while (elapsed < RotateToTargetDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / RotateToTargetDuration);

            characterController.enabled = false;
            player.transform.rotation = Quaternion.Slerp(startPlayerRot, targetPlayerRot, t);
            characterController.enabled = true;

            Vector3 dirToTargetLive = (faceTarget.position - cameraTransform.position).normalized;
            Quaternion targetWorldCamRotLive = Quaternion.LookRotation(dirToTargetLive, Vector3.up);
            Quaternion targetCamLocalRotLive = Quaternion.Inverse(player.transform.rotation) * targetWorldCamRotLive;

            cameraTransform.localRotation = Quaternion.Slerp(startCamRot, targetCamLocalRotLive, t);

            yield return null;
        }

        characterController.enabled = false;
        player.transform.rotation = targetPlayerRot;
        characterController.enabled = true;

        isFacingTarget = true;
        Debug.Log(isFacingTarget);

        // -------------------------------------------------------------------
        // PHASE 2: Continuous tracking. Runs indefinitely - keeps recomputing
        // the camera's look-at rotation against the target's live position
        // every frame, so head movement (dancing, looking around, etc.) is
        // followed in real time. This only stops when ReleaseFromTarget()
        // calls StopCoroutine on this routine.
        // -------------------------------------------------------------------
        while (true)
        {
            if (_currentFaceTarget != null)
            {
                Vector3 dirToTargetTrack = (_currentFaceTarget.position - cameraTransform.position).normalized;
                Quaternion targetWorldCamRotTrack = Quaternion.LookRotation(dirToTargetTrack, Vector3.up);
                Quaternion targetCamLocalRotTrack = Quaternion.Inverse(player.transform.rotation) * targetWorldCamRotTrack;

                cameraTransform.localRotation = Quaternion.Slerp(
                    cameraTransform.localRotation,
                    targetCamLocalRotTrack,
                    Time.deltaTime * HeadTrackSmoothSpeed
                );
            }

            yield return null;
        }
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