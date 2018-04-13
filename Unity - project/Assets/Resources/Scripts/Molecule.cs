﻿using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour
{

  private int numberOfAtoms;
  private int graspedAtoms;
  private int numberOfBonds;
  private bool pivotGrabbed;
  private bool isRotating, rotate, translate;
  private GameObject bond;
  private float bondScale;
  private float pivotOffset;
  private bool distanceToBond;

  public GameObject simpleBond;
  public GameObject doubleBond;
  public GameObject tripleBond;
  public GameObject quadrupleBond;

  private Vector3 axis;

  public GameObject rotationToogle;
  private GameObject pivot;
  private GameObject handController;
  private GameObject camera;
  private Vector3 initpos;

  private int bondType;
  private bool freeHand;
  private bool MultipleLines;
  
  //variables to set atoms bonds with distance
  private bool adjustingDistance;

  // Use this for initialization
  void Start()
  {
    pivotOffset = 0.1f;
    numberOfBonds = 1;
    numberOfAtoms = 2;
    graspedAtoms = 0;
    pivotGrabbed = false;
    isRotating = false;
    rotate = false;
    camera = GameObject.FindGameObjectWithTag("MainCamera");
    freeHand = camera.GetComponent<Manager>().freeHandRotation;
    MultipleLines = camera.GetComponent<Manager>().MultipleLines;
    distanceToBond = camera.GetComponent<Manager>().DistanceToBond;
    pivot = Instantiate(rotationToogle, transform.position, transform.rotation);
    pivot.transform.parent = transform;
    SetManagerPivot();
    UpdateStructure();
  }

  public void SetHandController(GameObject handCtrl)
  {
    handController = handCtrl;
  }

  void SetManagerPivot()
  {
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Interactable"))
      {
        pivot.transform.GetComponent<InteractionBehaviour>().manager = child.GetComponent<InteractionBehaviour>().manager;
        return;
      }
    }
  }

  // Update is called once per frame
  void Update()
  {
    CheckAtomsGrasped();
    UpdateStructure();
    graspedAtoms = 0;
    pivotGrabbed = false;
    
    CheckRotate();
    CheckTranslate();
    
    if(adjustingDistance)
      CheckDistance();
  }

  void CheckTranslate ()
  {
    if (translate) {
      Vector3 v = handController.GetComponent<HandController> ().GetHandMovement ();
      transform.position -= v; 
    }
  }

  void CheckRotate()
  {
    Vector3 pointRotate = pivot.transform.position;
    pointRotate.y += pivotOffset;
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Interactable") && rotate)
      {
        if(!freeHand)
          child.RotateAround(pointRotate, Vector3.forward, handController.GetComponent<HandController>().GetRotationSign()* 80 * Time.deltaTime);//handController.GetComponent<HandController>().GetHandRotation()
        else
          child.RotateAround(pointRotate, Vector3.forward, handController.GetComponent<HandController>().GetHandRotation());
      } 
      else if (child.CompareTag("Interactable") && !rotate)
        child.RotateAround(pointRotate, axis, 0);
    }
  }
  
  void CheckDistance ()
  {
    
  }

  //Count atoms touched in order to perform a dettachment
  void CheckAtomsGrasped()
  {
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Interactable") && child.GetComponent<InteractionBehaviour>().isGrasped)
      {
        graspedAtoms++;
      }
      else if (child.CompareTag("Pivot") && child.GetComponent<InteractionBehaviour>().isGrasped)
        pivotGrabbed = true;
    }
    CheckMoleculeState();
  }

  void CheckMoleculeState()
  {
    //If 2 atoms are grasped, then you start detaching
    if (graspedAtoms >= 2)
    {
      for (int i = 0; i < transform.childCount; i++)
      {
        Transform child = transform.GetChild(i);
        if (child.CompareTag("Bond"))
        {
          child.GetComponent<BondController>().CheckDetaching();
        }
      }
    }

    //If only one atom is grasped, then you are just moving it, and stop all other actions
    else if (graspedAtoms <= 1)
    {
      for (int i = 0; i < transform.childCount; i++)
      {
        Transform child = transform.GetChild(i);
        if (child.CompareTag("Bond"))
        {
          child.GetComponent<BondController>().StopDettaching();
        }
      }
    }

    if (!pivotGrabbed)
    {
      isRotating = false;
      for (int i = 0; i < transform.childCount; i++)
      {
        Transform child = transform.GetChild(i);
        if (child.CompareTag("Interactable"))
          child.GetComponent<Atom>().SetRotating(false);
      }
    }
  }

  void LockDistanceToPivot()
  {
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Interactable"))
      {
        float dist = Vector3.Distance(pivot.transform.position, child.position);
        child.GetComponent<Atom>().SetDistanceToPivot(pivot.transform.position);
      }
    }
  }

  public void SetAxis(bool endZGrabbed, bool endXGrabbed)
  {
    if (endZGrabbed)
    {
      axis = Vector3.forward;
    }
    if (endXGrabbed)
    {
      axis = Vector3.right;
    }
    if (!endZGrabbed && !endXGrabbed)
      axis = Vector3.zero;
  }

  public void AddBond(GameObject bond)
  {
    numberOfBonds++;
    bond.GetComponent<BondController>().SetID(numberOfBonds);
  }

  //Check what type of bond must be created
  void DefineBondType (GameObject atomA, GameObject atomB)
  {
    //get available bonds from the atoms
    int availableBondsA = atomA.GetComponent<Atom> ().GetAvailableBonds ();
    int availableBondsB = atomB.GetComponent<Atom> ().GetAvailableBonds ();
    if (availableBondsA <= 0 || availableBondsB <= 0)
      return;

    bondType = 0;
    
    //get the smallest bond type possible between the two atoms
    if (!distanceToBond) {
      if (availableBondsA < availableBondsB)
        bondType = availableBondsA;
      else
        bondType = availableBondsB;    
    } else {
      adjustingDistance = true;
    }
    
    
    //get the prefab for the given bond type
    if (MultipleLines)
    {
      switch (bondType)
      {
        case 1:
          bond = simpleBond;
          break;
        case 2:
          bond = doubleBond;
          break;
        case 3:
          bond = tripleBond;
          break;
        case 4:
          bond = quadrupleBond;
          break;
      }
      bondScale = 0;
    }
    else
    {
      bond = simpleBond;
      if (bondType == 2) bondScale = 0.02f;
      else if (bondType == 3) bondScale = 0.03f;
    }
  }

  //Create a new bond between the given atoms
  public void CreateBond(GameObject atomA, GameObject atomB)
  {
    DefineBondType(atomA, atomB);
    GameObject newBond = Instantiate(bond, transform.position, transform.rotation);
    AddBond(newBond);
    newBond.GetComponent<BondController>().SetAtoms(atomA, atomB, bondType);
    newBond.transform.localScale += new Vector3(bondScale,bondScale,0);
    newBond.transform.parent = transform;
  }


  //Destroy the molecule once there is no atoms bonded
  void UpdateStructure()
  {
    if (transform.childCount <= 1)
    {
      Destroy(pivot);
      Destroy(gameObject);
      return;
    }

    if (!isRotating)
    {
      Vector3 center = new Vector3(0, 0, 0);
      Vector3 lastPosition = new Vector3(0,0,0);
      int count = 0;
      for (int i = 0; i < transform.childCount; i++)
      {
        Transform child = transform.GetChild(i);
        if (child.CompareTag("Interactable"))
        {
          count++;
          center += (child.transform.position);
        }
      }

      Vector3 pivotPosition = center / count;
      pivotPosition.y -= pivotOffset;
      pivot.transform.position = pivotPosition;

    }
  }

  public bool CheckAtomsAreBonded(GameObject atomA, GameObject atomB)
  {
    bool bonded = false;
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Bond"))
      {
        bonded = child.GetComponent<BondController>().CheckAtomsBond(atomA, atomB);
      }
      if (bonded) return bonded;
    }
    return bonded;
  }

  public void CheckOtherBonds(Transform atom, int bondID)
  {
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Bond") && child.GetComponent<BondController>().getBondID() != bondID)
      {
        child.GetComponent<BondController>().CompareAtom(atom);
      }
    }
  }

  public void Rotate()
  {
    rotate = true;
  }

  public void StopRotate()
  {
    rotate = false;
  }

  public void Translate()
  {
    translate = true;
  }

  public void StopTranslate()
  {
    translate = false;
  }

}
