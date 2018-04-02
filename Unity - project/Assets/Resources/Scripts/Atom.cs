﻿using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour {

  public GameObject molecule;
  public GameObject bond;
  public GameObject handController;
  public Leap.Unity.Interaction.InteractionManager manager;
  //public float distance; //At which they stick together

  private string atomType = "Hydrogen";
  private bool grabbed;
  private int numberOfBondsAllowed = 0;
  private bool isAttached;
  private bool toDetach;
  private int numberOfBonds; //bonds that the atom has
  private GameObject nearAtom;
  private Material highlightMat;
  private Material highlightGrasp;
  private bool leftPinch;
  private bool rightPinch;
  private bool canBond;
  private List<int> bondIDs;

  private Vector3 pointToRotate;
  private bool isRotating;
  private float distanceToPivot;
  private Vector3 pivotPosition;

  // Use this for initialization
  void Start ()
  {
    distanceToPivot = 0;
    isRotating = false;
    isAttached = false;
    toDetach = false;
    highlightMat = transform.GetComponent<MeshRenderer>().materials[1];
    highlightGrasp = transform.GetComponent<MeshRenderer>().materials[2];
    handController = GameObject.FindGameObjectWithTag("HandController");
  }


  public void SetProperties(string type, Material mat, int bonds)
  {
    numberOfBonds = 0;
    numberOfBondsAllowed = bonds;
    bondIDs = new List<int>();
    atomType = type;
    transform.GetComponent<MeshRenderer>().material = mat;
    float size = Properties.SIZES[atomType];
    transform.localScale = new Vector3(size,size,size);
  }

  // Update is called once per frame
  void Update ()
  {
    //leftPinch = handController.GetComponent<HandController>().IsLeftPiching();
    //rightPinch = handController.GetComponent<HandController>().IsRightPiching();

    if (GetComponent<InteractionBehaviour>().isGrasped)
    {
      highlightGrasp.SetFloat("_Outline", 0.015f);
    }
    else
    {
      highlightGrasp.SetFloat("_Outline", 0.00f);
    }

    if (isRotating)
      Rotate();

    if (numberOfBonds >= 1)
      Attach();
    else Dettach();
  }

  public void SetDistanceToPivot(Vector3 pivotPos)
  {
    pivotPosition = pivotPos;
    distanceToPivot = Vector3.Distance(pivotPosition,transform.position);
  }

  public void EnableBond()
  {
    canBond = true;
  }

  public void DisableBond()
  {
    canBond = false;
  }

  public void SetRotation(Vector3 point, Vector3 axis, float angle)
  {
    transform.RotateAround(point, axis, angle);    
  }

  void Rotate()
  {
    transform.position = (transform.position - pivotPosition).normalized * distanceToPivot + pivotPosition;
  }

  public void SetRotating(bool rotate)
  {
    isRotating = rotate;
  }

  void Dettach()
  {
    transform.parent = null;
    isAttached = false;
    highlightMat.SetFloat("_Outline", 0f);
  }

  public void Attach()
  {
    isAttached = true;
  }

  public void ToDettach()
  {
    toDetach = true;    
  }

  public void StopDettach()
  {
    toDetach = false;
  }

  public bool CanBond()
  {
    return canBond;
  }

  public string GetAtomType()
  {
    return atomType;
  }

  public List<int> GetBonds()
  {
    return bondIDs;
  }

  public void SetBonds(List<int> bonds)
  {
    for(int i = 0; i < bonds.Count; i++)
    {
      bondIDs.Add(bonds[i]);
    }
  }

  public int GetNumberBondsMade()
  {
    return numberOfBonds;
  }

  public bool HasFreeBonds()
  {
    return numberOfBonds < numberOfBondsAllowed;
  }

  void OnCollisionEnter(Collision col)
  {
    if (col.transform.CompareTag("Interactable") && canBond && col.transform.GetComponent<Atom>().CanBond())
    {
      if(HasFreeBonds() && col.transform.GetComponent<Atom>().HasFreeBonds())
        StickToMolecule(col.gameObject);
    }
  }

  void StickToMolecule(GameObject obj)
  {

    if(obj.transform.parent == null && transform.parent == null)
    {
      Vector3 platePos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
      GameObject mole = Instantiate(molecule, platePos,transform.rotation);
      mole.GetComponent<Molecule>().SetHandController(handController);
      transform.parent = mole.transform;
      obj.transform.parent = mole.transform;
      mole.GetComponent<Molecule>().CreateBond(obj,transform.gameObject);
    }
    else if(obj.transform.parent != transform.parent)
    {
      if (transform.parent == null ) { transform.parent = obj.transform.parent; }
      else if (obj.transform.parent == null ) {  obj.transform.parent = transform.parent; }
      transform.parent.GetComponent<Molecule>().CreateBond(obj,transform.gameObject);
    }
    
    highlightMat.SetFloat("_Outline", 0.01f);
  }

  public void AddBond(int type, int bondId)
  {
    bondIDs.Add(bondId);
    numberOfBonds += type;
  }

  public void RemoveBond(int type, int bondId)
  {
    bondIDs.Remove(bondId);
    numberOfBonds -= type;
    toDetach = false;
  }
}
