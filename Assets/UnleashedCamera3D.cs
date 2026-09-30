using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnleashedCamera3D : MonoBehaviour
{
    private MobileBlur blur;
    public Transform target,lookAt;

    public float followSmoothSpeed = 0.125f;
    public float lookSmoothSpeed = 5f;
    public float orbitDistance = 5.0f;
    public float orbitHeight = 2.0f;
    public float upwardOffset = 1.5f;
    public float distanceForCameraToMoveUp = 4.33f;
    public float distance_lerp;
    

    [SerializeField] private float minXAxisDeadzone = 0.05f;
    [SerializeField] float turnAngleThreshold = 60f;
    [SerializeField] float jumpFollowThreshold = 3f;

    [Header("Drifting Rotation")]
    public float driftCamAngle = 5;
    public float driftRotateSpeed = 2;

    public float quickStepFollowOffset;
    float QSoffset;
    private SonicController sonic;
    private Rigidbody sonicrb;

    private Vector3 desiredPosition;


    private Vector3 lastForward;
    Vector3 playerPos;
    private float currentYawOffset = 0f;


    [SerializeField] private float recenterDuration = 1f; // How long to recenter
    private float recenterTimer = 0f;

    Vector3 lastFWD;

    private float lookOg, follOg;
    private float orbitDistanceOG;

    private void Start()
    {
        sonic = target.GetComponentInParent<SonicController>();
        sonicrb = target.GetComponentInParent<Rigidbody>();
        blur = GetComponent<MobileBlur>();

        desiredPosition = target.position + target.forward * -orbitDistance + Vector3.up * orbitHeight;
        orbitDistanceOG = orbitDistance;
        lookOg = lookSmoothSpeed;
        follOg = followSmoothSpeed;
    }

    private void LateUpdate()
    {
        blur.enabled = sonic.boost;
        FollowPlayer();
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothSpeed * Time.deltaTime);
        
    }

    private void FixedUpdate()
    {
       // transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothSpeed * Time.deltaTime);
       
    }

    private void FollowPlayer()
    {
        float speed = sonicrb.velocity.magnitude;
        Vector3 toTarget = target.position - transform.position;
        Vector3 toTargetHorizontal = new Vector3(toTarget.x, 0, toTarget.z);
        float xAxisMovement = toTargetHorizontal.normalized.x;

        if (Mathf.Abs(xAxisMovement) < minXAxisDeadzone)
            toTargetHorizontal.x = 0;

        float distanceFromIdeal = Vector3.Distance(transform.position, target.position + target.forward * -orbitDistance);
        if (speed < 5f && distanceFromIdeal < 0.5f)
            return;


        Vector3 toTargetLook = lookAt.position - transform.position;
        Vector3 toTargetHorizontalLook = new Vector3(toTarget.x, sonic.isGrounded ? toTarget.y : 0, toTarget.z);

        if (speed < 20f)
        {
            
            ApplyLookAt(toTargetHorizontalLook);
            HandleStillMode();
            if(sonic.sonic_action == SonicController.Actions.spline)
            {
                 HandleMovingMode();
            }
        }
        else
        {
            if(sonic.sonic_action == SonicController.Actions.spline)
            {
                //ApplyLookAt(toTargetHorizontalLook);
                HandleMovingMode();
            }
            else
            HandleMovingMode();
        }

        
        ApplyCameraRotation(toTargetHorizontal);
        ApplyExtraDamping();
        HandleJumpYLock();
        HandleUpwardOffset();
        HandleBoostFOV();
        HandleSpecialStates();
        OrbitDistanceReset();

        lastForward = sonic.transform.forward;
    }

    private void ApplyLookAt(Vector3 horizontalDirection)
    {
        Quaternion lookRotation = Quaternion.LookRotation(horizontalDirection + Vector3.up * 0.1f);
 
        // Was a hardcoded Time.deltaTime lerp (ignored lookSmoothSpeed entirely).
        // Now driven by lookSmoothSpeed like every other rotation call, and frame-rate independent.
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, FrameIndependentLerpFactor(lookSmoothSpeed));
    }

    private void HandleStillMode()
    {
        followSmoothSpeed = Mathf.Lerp(followSmoothSpeed, 2, Time.deltaTime * 10f);
         float turnAngle = Vector3.Angle(lastForward, sonic.transform.forward);

        if (turnAngle > turnAngleThreshold)
        {
            recenterTimer = recenterDuration;
        }

        if (recenterTimer > 0f)
        {
            desiredPosition = 
                target.position + target.forward * -orbitDistance + Vector3.up * orbitHeight;
            lastFWD = Camera.main.transform.forward;

            recenterTimer -= Time.deltaTime;
        }
        else
        {
            desiredPosition =  target.position + lastFWD * -orbitDistance + Vector3.up * (orbitHeight - 0.8f);
        }
    }

    private void HandleMovingMode()
    {
       // Vector3 playerPos = target.position;
        if(sonic.isGrounded){
            followSmoothSpeed = Mathf.Lerp(followSmoothSpeed, !sonic.boost ?  /*7f*/ 10f  : 9f, Time.deltaTime * 10f);
        }
        else
        {
            followSmoothSpeed = Mathf.Lerp(followSmoothSpeed, !sonic.boost ?  7f  : 9f, Time.deltaTime * 10f);
        }
        followSmoothSpeed=sonic.sonic_action == SonicController.Actions.spline? 2: followSmoothSpeed;
        lookSmoothSpeed = Mathf.Lerp(lookSmoothSpeed, 7f, Time.deltaTime * 5f);

       

        playerPos = target.position;

        
        if (!sonic.isGrounded && sonic.jumpHeight < jumpFollowThreshold)
        {   playerPos.y = transform.position.y;}
        
        playerPos.x += QSoffset;
        desiredPosition = playerPos + target.forward * -orbitDistance + Vector3.up * orbitHeight;
        lastFWD = Camera.main.transform.forward;

        
    }

    private void ApplyCameraRotation(Vector3 horizontalDirection)
    {
          float verticalLookAmount = (sonic.isGrounded || sonic.jumpHeight > jumpFollowThreshold || sonicrb.velocity.y < -5)
        ? (target.position.y - transform.position.y)
        : -1.7f; // Disable vertical look during small jumps

        if(sonic.sonic_action != SonicController.Actions.drift)
        {
            Quaternion targetRot = Quaternion.LookRotation(horizontalDirection + Vector3.up * verticalLookAmount);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, FrameIndependentLerpFactor(lookSmoothSpeed));

            
            if(sonic.isGrounded && sonic.groundSlopeAngle > 35)
            {
                 Vector3 sonicEuler = lookAt.transform.eulerAngles;
            // Keep camera Y rotation as current, and match X and Z to Sonic
            Quaternion targetRotation = Quaternion.Euler(
                sonicEuler.x, // Pitch
                transform.eulerAngles.y, // Keep Yaw (Y axis) steady — or use sonicEuler.y if you want full sync
                sonicEuler.z  // Roll (if desired)
            );
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * lookSmoothSpeed/2);
            }
            else
            {

            }
            
        }
        else
        {
            DriftingMode();
        }
    }

    void DriftingMode()
    {  
        // Example input — replace with your own drift system
        float driftInput = sonic.drift_direction > 0 ? 1 : -1;

        // Apply smooth yaw offset
        float targetYaw = driftInput * driftCamAngle;
        currentYawOffset = Mathf.Lerp(currentYawOffset, targetYaw, driftRotateSpeed * Time.deltaTime);

        // Build rotation
        Quaternion lookRotation = Quaternion.LookRotation(lookAt.forward, Vector3.up);
        Quaternion targetRotation = lookRotation * Quaternion.Euler(0, currentYawOffset, 0);

        // Smoothly apply rotation using Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSmoothSpeed * Time.deltaTime);
        
        orbitDistance = 4f;
    }


    private void ApplyExtraDamping()
    {
        lookSmoothSpeed = (sonic.sonic_action == SonicController.Actions.drift)
            ? Mathf.Lerp(lookSmoothSpeed, 49, 13 * Time.deltaTime)
            : Mathf.Lerp(lookSmoothSpeed, lookOg, 13 * Time.deltaTime);
    }

    private void HandleJumpYLock()
    {
        if (!sonic.isGrounded && sonic.jumpHeight < 10)
        {
            desiredPosition.y = target.position.y;
        }
    }

    private void HandleUpwardOffset()
    {
        if (Vector3.Distance(transform.position, target.position) < distanceForCameraToMoveUp)
        {
            desiredPosition += Vector3.up * upwardOffset;
            lookSmoothSpeed = 2;
            followSmoothSpeed = 2;
        }
    }

    private void HandleBoostFOV()
    {
        if (sonic.boost)
        {
            orbitDistance = 2;
            lookSmoothSpeed = 12f;
            Camera.main.fieldOfView = 111;
        }
        else
        {
            if (Camera.main.fieldOfView > 64)
                //Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, 63.4f, distance_lerp * Time.deltaTime);
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 63.4f, Time.deltaTime * 2f);
        }

         if (sonic.sonic_col.current_action == SonicObejctInteraction.action.speedpad)
        {
            orbitDistance = 2;
            lookSmoothSpeed = 12f;
            Camera.main.fieldOfView = 111;
        }

        
    }

    private void HandleSpecialStates()
    {
        if (sonic.sonic_action == SonicController.Actions.drift)
        {
            lookSmoothSpeed = 7f;
        }
        else if (sonic.sonic_action == SonicController.Actions.q_step)
        {
            lookSmoothSpeed = 6f;
            followSmoothSpeed = 7;
            int inpSnap = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            QSoffset = inpSnap * quickStepFollowOffset;
            
        }
        else
        {
            QSoffset = Mathf.MoveTowards(QSoffset,0,4 * Time.deltaTime);
        }

        if (Vector3.Angle(sonic.transform.forward, sonic.localMove) >= 12)
        {
            lookSmoothSpeed = 6f;
        }
        
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    void OrbitDistanceReset()
    {
        
        orbitDistance = Mathf.MoveTowards(orbitDistance, (sonic.speed/4 < 5) ? orbitDistanceOG : 2f  , 5 * Time.deltaTime);
        
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(target.position, minXAxisDeadzone);
        }
    }
    private float FrameIndependentLerpFactor(float speed)
    {
        return 1f - Mathf.Exp(-speed * Time.deltaTime);
    }
}
