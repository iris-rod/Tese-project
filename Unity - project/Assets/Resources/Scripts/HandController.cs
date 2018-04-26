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
  private int rotationSign;

  //pointing at shelf
  private Vector3 indexPos, indexDir;
  private bool pointing;

  //detect hand movement
  private Vector3 lastHandPosition, lastHandNormal;
  private Vector3 positionMovement;
  private Vector3 lastMovementNorm, lastNormalNorm;
  private bool flipping, movementFinished;
  private bool canDetect;

  //testing variables
  private bool UpDown = true;
  private int rotationType;

  // Use this for initialization
  void Start()
  {
    movementFinished = false;
    flipping = false;
    pointing = false;
    canDetect = true;
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
    rotationType = transform.parent.GetComponent<Manager>().rotationType;
  }

  public void updateCurrentHand(Hand leapHand)
  {

      CheckFingersPosition(leapHand);
      //CheckHandMovement(leapHand); //use with notepad only
      if (rotating)
        Rotate(leapHand);
      if (translate)
        Translate(leapHand);

      if (leapHand.IsLeft && leapHand.IsPinching()) leftHandPiching = true;
      else if (leapHand.IsLeft && !leapHand.IsPinching()) leftHandPiching = false;

      if (!leapHand.IsLeft && leapHand.IsPinching()) rightHandPiching = true;
      else if (!leapHand.IsLeft && !leapHand.IsPinching()) rightHandPiching = false;

      if (leapHand.IsLeft) leftHand = leapHand;
      else rightHand = leapHand;

      if (leapHand.IsLeft) lastPalmPosition = leapHand.PalmPosition.ToVector3();
    
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
      movement = lastPalmPosition - hand.PalmPosition.ToVector3();//- lastPalmPosition;
    }
  }

  //Set angle to rotate from hand current and last rotation, and the axis it must rotate around
  void Rotate (Hand hand)
  {
    if (!hand.IsLeft) {    
      handRotation = Quaternion.Angle (hand.Rotation.ToQuaternion (), lastRotation.ToQuaternion ());
      if (handRotation == 180) {
        handRotation = 0;
      }
      if (hand.PalmNormal.x < lastPalmX)// || (hand.PalmNormal.x <0 && lastPalmX <0 && hand.PalmNormal.x > lastPalmX))
        handRotation = -handRotation;
      lastRotation = hand.Rotation;
      lastPalmX = hand.PalmNormal.x;
    }
    else if (hand.IsLeft)
    {
      CheckAxis(hand);
      pivotRotate.transform.parent.GetComponent<Molecule>().SetAxis(Z,X);
    }
  }

  //Check hand position to determine which axis the molecule rotates
  void CheckAxis (Hand hand)
  {
    if (fingerStreched) { // left hand is open
      if (UpDown) { //palma para cima ou para baixo
        if (hand.PalmNormal.y >= interval && hand.PalmNormal.y <= 1) {
          Z = true;
          X = false;
        } else if (hand.PalmNormal.y <= -interval && hand.PalmNormal.y >= -1) {
          Z = false;
          X = true;
        }
      } 
      else {
        Z = false;
        X = false;
      }
    }
    else if(!UpDown) 
    { 
      //palma para o lado e virada para tras
      if(hand.PalmNormal.x >= interval && hand.PalmNormal.x <= 1 && hand.PalmNormal.y >= -.2f && hand.PalmNormal.y <= .15f && hand.PalmNormal.z >= -.35f && hand.PalmNormal.z <= 0.05f) //x 1 y 0.1
      {
        Z = true;
        X = false;
      }
      else if (hand.PalmNormal.z <= -interval && hand.PalmNormal.z >= -1) //x 0 y 0.1
      {
        Z = false;
        X = true;
      }    
      else{
        Z = false;
        X = false;
      }  
    }
    else if(!fingerStreched && UpDown){
        Z = false;
        X = false;
    }    
    
  }


  /* Functions used on trigger of index finger streched**/
  public void SelectAxis()
  {
    if (pivotRotate != null && UpDown)
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
  
  
  void ResetDetect ()
  {
    canDetect = true;
  }

  //Verify the fingers position to determine what action to do (rotate or translate)
  void CheckFingersPosition(Hand hand)
  {
    if (!hand.IsLeft)
    {
      if (!rotating && rotationType != 3 && !hand.Fingers[4].IsExtended && !hand.Fingers[3].IsExtended)
      {
        BeginRotation();
      }
      else if (rotating && rotationType != 3 && (hand.Fingers[4].IsExtended || hand.Fingers[3].IsExtended))
      {
        StopRotation();
      }
      //pointing
      if (hand.Fingers[1].IsExtended && !hand.Fingers[0].IsExtended && !hand.Fingers[2].IsExtended && !hand.Fingers[3].IsExtended && !hand.Fingers[4].IsExtended)
      {
        pointing = true;
        indexPos = hand.Fingers[1].TipPosition.ToVector3();
        indexDir = hand.Fingers[1].Direction.ToVector3();
      }
      else
      {
        pointing = false;
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

    }
  }

  public void BeginTranslate()
  {
    for (int i = 0; i < pivots.Length; i++)
    {
      GameObject obj = pivots[i];
      if (obj != null && obj.CompareTag("Pivot") && obj.GetComponent<InteractionBehaviour>().isGrasped)
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
      if (obj != null && obj.CompareTag("Pivot") && obj.GetComponent<InteractionBehaviour>().isGrasped)
      {
        pivotRotate = obj;
        obj.transform.parent.GetComponent<Molecule>().Rotate();
        rotating = true;
        break;
      }
    }
  }

  public void StopRotation()
  {
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
    lastRotation = new Leap.LeapQuaternion();
    lastPalmX = 0;
  }

  public void GrabbedAtom()
  {
    atomsGrabbed++;
  }

  public int GetRotationSign()
  {
    return rotationSign;
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

  public Vector3 GetIndexPosition()
  {
    return indexPos;
  }
  public Vector3 GetIndexDirection()
  {
    return indexDir;
  }

}
