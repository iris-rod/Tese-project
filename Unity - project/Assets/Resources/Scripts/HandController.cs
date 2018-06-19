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
  private Vector3  movement;
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

  //rotation variables
  private bool rightHandGrabbingPivot;

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
    rotationType = transform.parent.transform.parent.GetComponent<Manager>().rotationType;//transform.parent.GetComponent<Manager>().rotationType;
  }

  public void updateCurrentHand (Hand leapHand)
  {

    rotationType = transform.parent.transform.parent.GetComponent<Manager> ().rotationType; 
    UpdatePivots();


    CheckGraspingPivot();
    CheckDistanceToPivot(leapHand);
    CheckAxis(leapHand);
 

    if (!leftHandGO.activeSelf || !rightHandGO.activeSelf) {
      StopRotation ();
    }
    if ((!leftHandGO.activeSelf && !rightHandGrabbingPivot) || (!rightHandGO.activeSelf && rightHandGrabbingPivot))
    {
      rightHandGrabbingPivot = !rightHandGrabbingPivot;
      StopTranslate();
    }


    //so roda se as duas maos estiverem a aparecer
    if (leftHandGO.activeSelf && rightHandGO.activeSelf) {
      if (rotating && !translate)
        Rotate (leapHand);
    }
      if (translate && !rotating)
        Translate (leapHand);

    if (leapHand.IsLeft)
    {
      leftHandPosition = leapHand.PalmPosition.ToVector3();
      leftHand = leapHand;
    }
    else
    {
      rightHandPosition = leapHand.PalmPosition.ToVector3();
      rightHand = leapHand;
    }
  }

  void Update()
  {
    
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
  
  void UpdatePivots ()
  {
  
    GameObject[] pivotsInScene = GameObject.FindGameObjectsWithTag ("Pivot");
    int length = 0;
    for (int i = 0; i < pivotsInScene.Length; i++) {
      if (pivotsInScene [i].transform.parent.name == "MoleculeV3(Clone)")
        length++;
    }
    pivots = new GameObject[length];
    for (int i = 0, n = 0; i < pivotsInScene.Length; i++) {
      if (pivotsInScene [i].transform.parent.name == "MoleculeV3(Clone)") {
        pivots [n] = pivotsInScene [i];
        n++;
      }
    }   
    
  }

  // check which hand is grabbing the pivot
  void CheckGraspingPivot ()
  {
    for (int i = 0; i < pivots.Length; i++) {
      GameObject pivot = pivots [i];
      if (pivot != null && pivot.GetComponent<InteractionBehaviour> ().isGrasped) {
        float leftDist = 100;
        float rightDist = 100;
        if (leftHandGO != null)
          leftDist = Vector3.Distance (leftHandPosition, pivot.transform.position);
        if (rightHandGO != null)
          rightDist = Vector3.Distance (rightHandPosition, pivot.transform.position);

        if (rightDist < leftDist)
          pivot.GetComponent<PivotController> ().SetGrasped (2);
        else
          pivot.GetComponent<PivotController> ().SetGrasped (1);
      } else {
          pivot.GetComponent<PivotController> ().SetGrasped (0);
      }
    }
  }

  // avoid pivot being grabbed when the hand is to far from it
  void CheckDistanceToPivot (Hand hand)
  {
    float maxDist = .1f;
    for (int i = 0; i < pivots.Length; i++) {
      GameObject pivot = pivots [i];
      if ((hand.IsLeft && pivot.GetComponent<PivotController> ().IsGrasped () == 1) || (hand.IsRight && pivot.GetComponent<PivotController> ().IsGrasped () == 2)) {
        if (pivot != null && pivot.GetComponent<InteractionBehaviour> ().isGrasped) {
          if (Vector3.Distance (pivot.transform.position, hand.PalmPosition.ToVector3 ()) > maxDist) {
            pivot.GetComponent<InteractionBehaviour> ().ignoreGrasping = true;
            pivot.GetComponent<PivotController> ().SetGrasped (0);
            StopTranslate ();
            StopRotation ();
            grabbedPivot = pivot;
            Z = false;
            X = false;
            Invoke ("ResetGrasp", 0.5f);
          } else if (hand.IsRight) {
            //a mao direita pode ser usada para rodar ou fazer translaçao, por isso so roda se nenhum eixo estiver activo
            if (!Z && !X)
            {
              rightHandGrabbingPivot = true;
              StopRotation();
              BeginTranslate();
            }
            else
            {
              StopTranslate();
              BeginRotation();
            }
          } else if (hand.IsLeft)
          {
            rightHandGrabbingPivot = false;
            StopRotation ();
            BeginTranslate ();
          }
        } 
      } else if(pivot.GetComponent<PivotController> ().IsGrasped () == 0) {
        StopTranslate ();
        StopRotation ();
        Z = false;
        X = false;
      }
    }
  }
  
  void ResetGrasp(){
    grabbedPivot.GetComponent<InteractionBehaviour> ().ignoreGrasping = false;
  }

  void Translate(Hand hand)
  {
    if (hand.IsLeft && !rightHandGrabbingPivot)
    {
      movement = leftHandPosition - hand.PalmPosition.ToVector3();//- lastPalmPosition;
    }
    else if (hand.IsRight && rightHandGrabbingPivot)
    {
      movement = rightHandPosition - hand.PalmPosition.ToVector3();//- lastPalmPosition;
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
      pivotRotate.transform.parent.GetComponent<Molecule>().SetAxis(Z,X);
    }
  }

  //Check hand position to determine which axis the molecule rotates
  void CheckAxis (Hand hand)
  {
    if (fingerStreched && hand.IsLeft) { // left hand is open
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
    else if(!fingerStreched && UpDown){
        Z = false;
        X = false;
    }
    if(X || Z)
      leftHandGO.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_Outline", 0.05f);
    else
      leftHandGO.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_Outline", 0.0f);
  }


  /* Functions used on trigger of index finger streched**/
  public void SelectAxis()
  {
    if(UpDown)
      fingerStreched = true;
  }

  public void DeselectAxis()
  {
    fingerStreched = false;
  }
  
  
  void ResetDetect ()
  {
    canDetect = true;
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
        //rightHandGO.transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.025f);
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
        //rightHandGO.transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.0f);
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
