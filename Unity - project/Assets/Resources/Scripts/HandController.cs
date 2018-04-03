using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;

public class HandController : MonoBehaviour
{
  Hand currentHand;
  private GameObject[] interactableObjs, pivots, axis;
  private bool leftHandPiching, rightHandPiching;
  private Hand leftHand, rightHand;

  private int atomsGrabbed;
  private bool rotating, translate;
  private Vector3 lastPalmPosition, movement;
  private LeapQuaternion lastRotation;
  private float handRotation;
  private float lastPalmX, lastPalmZ;
  private GameObject pivotRotate;
  private bool palmUp;
  private float interval;
  private bool Z, X, fingerStreched;
  private bool pointing;

  //testing variables
  private bool UpDown = false;

  // Use this for initialization
  void Start()
  {
    pointing = false;
    interval = 0.90f;
    handRotation = 0;
    atomsGrabbed = 0;
    lastPalmX = 0;
    lastPalmZ = 0;
    rotating = false;
    translate = false;
    leftHandPiching = false;
    rightHandPiching = false;
    interactableObjs = GameObject.FindGameObjectsWithTag("Interactable");
    pivots = GameObject.FindGameObjectsWithTag("Pivot");
  }

  public void updateCurrentHand(Hand leapHand)
  {
    CheckFingersPosition(leapHand);
    if (rotating)
      Rotate(leapHand);
    if (translate)
      Translate(leapHand);

    if (leapHand.IsLeft && leapHand.IsPinching()) leftHandPiching = true;
    else if (leapHand.IsLeft && !leapHand.IsPinching()) leftHandPiching = false;

    if (!leapHand.IsLeft && leapHand.IsPinching()) rightHandPiching = true;
    else if (!leapHand.IsLeft && !leapHand.IsPinching()) rightHandPiching = false;
    //Quaternion.Euler(leapHand.Direction.Pitch, leapHand.Direction.Yaw, leapHand.PalmNormal.Roll);

    if (leapHand.IsLeft) leftHand = leapHand;
    else rightHand = leapHand;
  }

  void Update()
  {

    //testing 
    CheckKeyboard();

    interactableObjs = GameObject.FindGameObjectsWithTag("Interactable");
    pivots = GameObject.FindGameObjectsWithTag("Pivot");

    int grabbedAtoms = 0;
    for (int i = 0; i < interactableObjs.Length; i++)
    {
      GameObject obj = interactableObjs[i];
      if (obj.GetComponent<InteractionBehaviour>().isGrasped)
      {
        grabbedAtoms++;
      }
    }
    if (grabbedAtoms >= 2)
    {
      for (int i = 0; i < interactableObjs.Length; i++)
      {
        GameObject obj = interactableObjs[i];
        if (obj.GetComponent<InteractionBehaviour>().isGrasped)
          obj.GetComponent<Atom>().EnableBond();
      }
    }
    else
    {
      for (int i = 0; i < interactableObjs.Length; i++)
      {
        GameObject obj = interactableObjs[i];
        obj.GetComponent<Atom>().DisableBond();
      }
    }
  }

  void CheckKeyboard()
  {
    if (Input.GetKeyDown("u"))
    {
      UpDown = true;
    }
    if (Input.GetKeyDown("i"))
    {
      UpDown = false;
    }
  }

  void Translate(Hand hand)
  {
    if (hand.IsLeft)
    {
      movement = hand.PalmPosition.ToVector3();//- lastPalmPosition;
      lastPalmPosition = hand.PalmPosition.ToVector3();
    }
  }

  //Set angle to rotate from hand current and last rotation, and the axis it must rotate around
  void Rotate(Hand hand)
  {
    if (!hand.IsLeft)
    {
      handRotation = Quaternion.Angle(hand.Rotation.ToQuaternion(), lastRotation.ToQuaternion());
      if (hand.PalmNormal.x < lastPalmX)
        handRotation = -handRotation;
      lastRotation = hand.Rotation;
      lastPalmX = hand.PalmNormal.x;
      lastPalmZ = hand.PalmNormal.z;
      if (pivotRotate.GetComponent<PivotController>().Axis)
      {
        bool endZ = pivotRotate.GetComponent<PivotController>().IsEndZGrabbed();
        bool endX = pivotRotate.GetComponent<PivotController>().IsEndXGrabbed();
        pivotRotate.transform.parent.GetComponent<Molecule>().SetAxis(endZ, endX);
      }
    }
    else if (hand.IsLeft && !pivotRotate.GetComponent<PivotController>().Axis)
    {
      CheckAxis(hand);
      pivotRotate.transform.parent.GetComponent<Molecule>().SetAxis(Z,X);
    }
  }

  //Check hand position to determine which axis the molecule rotates
  void CheckAxis(Hand hand)
  {
    if (fingerStreched) // left hand is open
    {
      if (UpDown)
      {
        if (hand.PalmNormal.y >= interval && hand.PalmNormal.y <= 1)
        {
          Z = true;
          X = false;
        }
        else if (hand.PalmNormal.y <= -interval && hand.PalmNormal.y >= -1)
        {
          Z = false;
          X = true;
        }
      }else
      {
      //Debug.Log(hand.PalmNormal);
        if(hand.PalmNormal.x >= interval && hand.PalmNormal.x <= 1 && hand.PalmNormal.y >= -.2f && hand.PalmNormal.y <= .15f) //x 1 y 0.1
        {
          Z = true;
          X = false;
        }
        else if (hand.PalmNormal.z <= -interval && hand.PalmNormal.z >= -1) //x 0 y 0.1
        {
          Z = false;
          X = true;
        }
      }
    }
    else
    {
      Z = false;
      X = false;
    }
  }


  /* Functions used on trigger of index finger streched**/
  public void SelectAxis()
  {
    if (pivotRotate != null)
    {
      fingerStreched = true;
    }
  }

  public void DeselectAxis()
  {
    if (pivotRotate != null)
    {
      fingerStreched = false;
    }
  }
  

  //Verify the fingers position to determine what action to do (rotate or translate)
  void CheckFingersPosition(Hand hand)
  {
    if (!hand.IsLeft)
    {
      if (!rotating && !hand.Fingers[4].IsExtended && !hand.Fingers[3].IsExtended)
      {
        BeginRotation();
      }
      else if (rotating && (hand.Fingers[4].IsExtended || hand.Fingers[3].IsExtended))
      {
        StopRotation();
      }
    }
    else
    {
      if (!translate && !hand.Fingers[4].IsExtended && !hand.Fingers[3].IsExtended)
      {
        BeginTranslate();
      }
      else if (translate && (hand.Fingers[4].IsExtended || hand.Fingers[3].IsExtended))
      {
        StopTranslate();
      }
      if(hand.Fingers[1].IsExtended && !hand.Fingers[0].IsExtended && !hand.Fingers[2].IsExtended && !hand.Fingers[3].IsExtended && !hand.Fingers[4].IsExtended)
      {
        pointing = true;
      }else
      {
        pointing = false;
      }
    }
  }

  public void BeginTranslate()
  {
    for (int i = 0; i < pivots.Length; i++)
    {
      GameObject obj = pivots[i];
      if (obj.CompareTag("Pivot") && obj.GetComponent<InteractionBehaviour>().isGrasped)
      {
        obj.transform.parent.GetComponent<Molecule>().Translate();
        translate = true;
        break;
      }
    }
  }

  public void StopTranslate()
  {
    for (int i = 0; i < pivots.Length; i++)
    {
      GameObject obj = pivots[i];
      if (obj.CompareTag("Pivot"))
      {
        obj.transform.parent.GetComponent<Molecule>().StopTranslate();
        break;
      }
    }
    translate = false;

  }

  public void BeginRotation()
  {
    for (int i = 0; i < pivots.Length; i++)
    {
      GameObject obj = pivots[i];
      if (obj.CompareTag("Pivot") && obj.GetComponent<InteractionBehaviour>().isGrasped)
      {
        pivotRotate = obj;
        bool endZ = obj.GetComponent<PivotController>().IsEndZGrabbed();
        bool endX = obj.GetComponent<PivotController>().IsEndXGrabbed();
        obj.transform.parent.GetComponent<Molecule>().SetAxis(endZ,endX);
        obj.transform.parent.GetComponent<Molecule>().Rotate();
        rotating = true;
        break;
      }
    }
  }

  public void StopRotation()
  {
    //Debug.Log("stop");
    for (int i = 0; i < pivots.Length; i++)
    {
      GameObject obj = pivots[i];
      if (obj.CompareTag("Pivot"))
      {
        obj.transform.parent.GetComponent<Molecule>().StopRotate();
        break;
      }
    }
    rotating = false;
  }

  public void GrabbedAtom()
  {
    atomsGrabbed++;
  }

  public Vector3 GetHandMovement()
  {
    return movement;
  }

  public float GetHandRotation()
  {
    return handRotation;
  }

  public Hand GetLeftHand()
  {
    return leftHand;
  }

  public Hand GetRightHand()
  {
    return rightHand;
  }


  public bool IsLeftPiching()
  {
    return leftHandPiching;
  }

  public bool IsRightPiching()
  {
    return rightHandPiching;
  }

  public bool IsPointing()
  {
    return pointing;
  }

}
