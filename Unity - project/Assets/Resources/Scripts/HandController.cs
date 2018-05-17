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
  private Vector3 leftHandPosition, rightHandPosition;

  //detect hand movement
  private Vector3 lastHandPosition, lastHandNormal;
  private Vector3 positionMovement;
  private Vector3 lastMovementNorm, lastNormalNorm;
  private bool canDetect;
  private bool lastHandRight;
  private GameObject grabbedPivot; // stop grasping when the hand is too far away from the GO
  public GameObject leftHandGO;
  public GameObject rightHandGO;

  //testing variables
  private bool UpDown = true;
  private int rotationType;

  // Use this for initialization
  void Start()
  {
    lastHandRight = false;
    canDetect = true;
    interval = 0.90f;
    handRotation = 0;
    atomsGrabbed = 0;
    lastPalmX = 0;
    lastPalmZ = 0;
    rotating = false;
    translate = false;
    interactableObjs = GameObject.FindGameObjectsWithTag("Interactable");
    pivots = GameObject.FindGameObjectsWithTag("Pivot");
    rotationType = transform.parent.GetComponent<Manager>().rotationType;//transform.parent.GetComponent<Manager>().rotationType;
  }

  public void updateCurrentHand (Hand leapHand)
  {
    rotationType = transform.parent.GetComponent<Manager> ().rotationType;
    CheckFingersPosition (leapHand);    
    pivots = GameObject.FindGameObjectsWithTag ("Pivot");


    if (!leftHandGO.activeSelf || !rightHandGO.activeSelf) {
      StopRotation ();
    }
    if (!leftHandGO.activeSelf)
      StopTranslate ();

    //so roda se as duas maos estiverem a aparecer
    if (leftHandGO.activeSelf && rightHandGO.activeSelf) {
      if (rotating && !translate)
        Rotate (leapHand);
    }
      if (translate && !rotating)
        Translate (leapHand);

      if (leapHand.IsLeft)
        leftHand = leapHand;
      else
        rightHand = leapHand;

    if (leapHand.IsLeft)
    {
      lastPalmPosition = leapHand.PalmPosition.ToVector3();
      leftHandPosition = leapHand.PalmPosition.ToVector3();
    }
    else
    {
      rightHandPosition = leapHand.PalmPosition.ToVector3();
    }
    CheckGraspingPivot();
    CheckDistanceToPivot(leapHand);
  }

  void Update()
  {
    //testing 
    CheckKeyboard();

    interactableObjs = GameObject.FindGameObjectsWithTag("Interactable");

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

  // check which hand is grabbing the pivot
  void CheckGraspingPivot()
  {
    for(int i = 0; i < pivots.Length; i++)
    {
      GameObject pivot = pivots[i];
      if(pivot != null && pivot.GetComponent<InteractionBehaviour>().isGrasped)
      {
        float leftDist = 100;
        float rightDist = 100;
        if (leftHandGO != null)
          leftDist = Vector3.Distance(leftHandPosition, pivot.transform.position);
        if(rightHandGO != null)
          rightDist = Vector3.Distance(rightHandPosition, pivot.transform.position);

        if (rightDist < leftDist)
          pivot.GetComponent<PivotController>().SetGraspedByLeft(false);
        else
          pivot.GetComponent<PivotController>().SetGraspedByLeft(true);
      }
    }
  }

  // avoid pivot being grabbed when the hand is to far from it
  void CheckDistanceToPivot (Hand hand)
  {
    float maxDist = .1f;
    for (int i = 0; i < pivots.Length; i++) {
       GameObject pivot = pivots [i];
      if (hand.IsLeft == pivot.GetComponent<PivotController>().IsGraspedByLeft())
      {
        //Debug.Log(pivot.GetComponent<PivotController>().IsGraspedByLeft());
        if (pivot != null && pivot.GetComponent<InteractionBehaviour>().isGrasped)
        {
          if (Vector3.Distance(pivot.transform.position, hand.PalmPosition.ToVector3()) > maxDist)
          {
            pivot.GetComponent<InteractionBehaviour>().ignoreGrasping = true;
            grabbedPivot = pivot;
            Invoke("ResetGrasp", 0.5f);
          }
        }
      }
    }
  }
  
  void ResetGrasp(){
    grabbedPivot.GetComponent<InteractionBehaviour> ().ignoreGrasping = false;
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
    if (hand.IsRight && !translate)
    {
      if (!rotating && rotationType != 3 && !hand.Fingers[4].IsExtended && !hand.Fingers[3].IsExtended)
      {
        BeginRotation();
      }
      else if (rotating && rotationType != 3 && (hand.Fingers[4].IsExtended || hand.Fingers[3].IsExtended))
      {
        StopRotation();
      }
      
    }
    else if(hand.IsLeft && !rotating)
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
      if (obj != null)
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
      if (obj != null && obj.GetComponent<InteractionBehaviour>().isGrasped)
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
      if (obj != null )
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



}
