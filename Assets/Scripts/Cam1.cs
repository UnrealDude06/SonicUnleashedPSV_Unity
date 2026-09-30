using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam1 : MonoBehaviour {

  public Transform mPlayer;

  public Vector3 mPositionOffset = new Vector3(0.0f, 2.0f, -2.5f);
  public Vector3 mAngleOffset = new Vector3(0.0f, 0.0f, 0.0f);
  [Tooltip("The damping factor to smooth the changes " +
    "in position and rotation of the camera.")]
  public float mDamping = 1.0f;

  [Header("Follow Independent Rotation")]
  public float mMinPitch = -30.0f;
  public float mMaxPitch = 30.0f;
  public float mRotationSpeed = 5.0f;
  private float angleX = 0.0f;

  void Start()
  {
  }

  void LateUpdate()
  {
  if(Vector3.Distance(transform.position,mPlayer.transform.position) < 0.1f)
  {
        Quaternion rot = Quaternion.Lerp(transform.rotation,
          Quaternion.LookRotation(new Vector3(mPlayer.rotation.x,mPlayer.rotation.y,mPlayer.rotation.z)),
          Time.deltaTime * 122);

        transform.rotation = rot;

  }
  else
  {
    if(mPlayer.GetComponent<SonicController>().speed>= 5)
    {
      CameraMove_Follow(true);
      if(mPlayer.GetComponent<SonicController>().speed > 18)
      {
        
        mDamping = 6f;
      }
      else
      {
        
        mDamping = 3f;
      }
    }
    if(mPlayer.GetComponent<SonicController>().speed < 5 && (Vector3.Distance(transform.position,mPlayer.transform.position) > 0.1f))
    {
        CameraMove_Follow(false);
    }

  }
  }


  void CameraMove_Follow(bool allowRotationTracking = false)
  {
    // We apply the initial rotation to the camera.
    Quaternion initialRotation = Quaternion.Euler(mAngleOffset);

    // added the following code to allow rotation tracking of the player
    // so that our camera rotates when the player rotates and at the same
    // time maintain the initial rotation offset.
    if (allowRotationTracking)
    {
      Quaternion rot = Quaternion.Lerp(transform.rotation,
          mPlayer.rotation * initialRotation,
          Time.deltaTime * mDamping);

      transform.rotation = rot;
    }
    else
    {
      transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        initialRotation,
        mDamping * Time.deltaTime);
    }

    // Now we calculate the camera transformed axes.
    Vector3 forward = transform.rotation * Vector3.forward;
    Vector3 right = transform.rotation * Vector3.right;
    Vector3 up = transform.rotation * Vector3.up;

    // We then calculate the offset in the 
    // camera's coordinate frame.
    Vector3 targetPos = mPlayer.position;
    Vector3 desiredPosition = targetPos
        + forward * mPositionOffset.z
        + right * mPositionOffset.x
        + up * mPositionOffset.y;

    // Finally, we change the position of the camera, 
    // not directly, but by applying Lerp.
    Vector3 position = Vector3.Lerp(transform.position,
        desiredPosition,
        Time.deltaTime * mDamping);

    transform.position = position;
  }

 
}