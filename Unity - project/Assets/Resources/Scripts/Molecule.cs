using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour
{
  private int ID;
  private int graspedAtoms;
  private int numberOfBonds;
  private bool pivotGrabbed;
  private bool canTranslate, rotate, translate, isRotating;
  private GameObject bond;
  private float bondScale;
  private float pivotOffset;
  private MoleculeManager MM;
  private int lastChildCount;

  public GameObject simpleBond;
  public GameObject doubleBond;
  public GameObject tripleBond;
  public GameObject quadrupleBond;
  public GameObject centerGO;
  public GameObject rotationToogle;


  private GameObject lastInviBond;
  private int lastInviBondType;
  private Vector3 axis;
  private GameObject pivot, centerRep;
  private GameObject handController;
  private GameObject camera;

  //variables picked up from the manager
  private int bondType;
  private int rotationType;

  //variables used to bond atoms
  private bool bondingAtoms, canUpdateTap;
  private GameObject atom1, atom2;
  private int numberOfTaps;

  //variables related to the mini molecules on the shelves
  private GameObject shelves;
  private bool isMini;
  private Vector3 hitScale;

  //variables center of the molecule
  private Vector3 center;

  //variables for rotation
  private float rotationOffset;
  private Transform grabbedChild;
  private Vector3 lastPosGrabbedChild;
  private float rotationValue;
  private bool canCenterInst;

  //variables for testing
  private int numberTesting;

  void Awake()
  {
    camera = GameObject.FindGameObjectWithTag("MainCamera");
    shelves = GameObject.Find("shelves");
    MM = GameObject.Find("GameManager").GetComponent<MoleculeManager>();
    rotationType = camera.GetComponent<Manager>().rotationType;
  }

  // Use this for initialization
  void Start ()
  {
    rotationOffset = 3.5f;
    pivotOffset = 0.12f;
    numberOfBonds = 1;
    numberOfTaps = 1;
    graspedAtoms = 0;
    lastChildCount = transform.childCount;
    hitScale = new Vector3 (.15f, .15f, .15f);//desktop -> new Vector3(0.03f, 0.1f, 0.1f);
    pivotGrabbed = false;
    rotate = false;
    isRotating = false;
    canUpdateTap = true;
    canCenterInst = true;
    pivot = Instantiate (rotationToogle, transform.position, transform.rotation);
    pivot.transform.parent = transform;
    SetManagerPivot ();
    UpdateStructure ();
    isMini = false;
    string split = transform.name.Split ('_') [0];
    
    if (split == "Mini") {
      isMini = true;
      pivotOffset = 0.04f;
      
      //disable interaction with saved molecules on the shelf
      for (int i = 0; i < transform.childCount; i++) {
        Transform child = transform.GetChild(i);
        if(child.CompareTag("Interactable") || child.CompareTag("Pivot"))
          child.GetComponent<InteractionBehaviour>().ignoreGrasping = true;
      }
      //MM.AddMolecule(transform.gameObject,true);
    }else
    {
      //MM.AddMolecule(transform.gameObject,false);
    }
    //disable all scripts in atoms of molecules used for testing 
    if (transform.name != "MoleculeV3(Clone)" && !isMini)
    {
      pivot.transform.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Pivot Invi", typeof(Material)) as Material;
      DisableScriptsAtoms();
    }

  }
  
  private void DisableScriptsAtoms ()
  {
    for (int i = 0; i < transform.childCount; i++) {
      Transform child = transform.GetChild (i);
      if (child.CompareTag ("Interactable")) {
        child.GetComponent<Atom> ().enabled = false;
        child.GetComponent<InteractionBehaviour> ().enabled = false;
        child.GetComponent<BondRepManager> ().enabled = false;
        child.GetComponent<SphereCollider> ().enabled = false;
      } else if (child.CompareTag ("Pivot")) {
        child.GetComponent<InteractionBehaviour> ().enabled = false;
        child.GetComponent<PivotController> ().enabled = false;
        child.GetComponent<SphereCollider> ().enabled = false;
      } else if (child.CompareTag ("Bond")) {
        //child.GetComponent<BondController> ().enabled = false;
      }
    }
   transform.gameObject.AddComponent(typeof(InvisibleMoleculeBehaviour));
   transform.GetComponent<InvisibleMoleculeBehaviour>().SetTask(transform.name.ToLower());
    transform.GetComponent<InvisibleMoleculeBehaviour>().SetNumber(numberTesting);

    transform.tag = "Invisible";
  }

  public void SetNumber(int n)
  {
    numberTesting = n;
  }

  public void SetHandController(GameObject handCtrl)
  {
    handController = handCtrl;
  }

  public void SetId(int value)
  {
    ID = value;
  }

  public int GetId()
  {
    return ID;
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
    if (!isMini)
    {
      CheckVariables();
      CheckAtomsGrasped();
      UpdateStructure();
      graspedAtoms = 0;
      pivotGrabbed = false;
      CheckRotate();
      CheckTranslate();
      if(bondingAtoms)
        CheckTaps();
    }
  }

  void CheckVariables()
  {
    rotationType = camera.GetComponent<Manager>().rotationType;
  }

  void CheckTaps()
  {
    SetInvisibleBond(numberOfTaps);
    if (!atom1.GetComponent<Atom>().IsBonding() && !atom2.GetComponent<Atom>().IsBonding())
      CreateBondTaps(numberOfTaps);
  }

  //creates the bond according to the distance to guide the user
  void SetInvisibleBond(float value)
  {
    bond = simpleBond;
    int bondNumber = 1;
      int n = Mathf.RoundToInt(value);
      switch (n)
      {
        case 1:
          bond = simpleBond;
          bondNumber = 1;
          break;
        case 2:
          bond = doubleBond;
          bondNumber = 2;
          break;
        case 3:
          bond = tripleBond;
          bondNumber = 3;
          break;
        case 4:
          bond = quadrupleBond;
          bondNumber = 4;
          break;
      }
    if (lastInviBondType != bondNumber)
    {
      if (lastInviBond != null)
        lastInviBond.GetComponent<FeedbackBondController>().DestroyBond(atom1, atom2);
      GameObject newBond = Instantiate(bond, transform.position, transform.rotation);
      Color color = newBond.GetComponent<MeshRenderer>().material.color;
      color.a = 0.3f;
      newBond.GetComponent<MeshRenderer>().material.color = color;
      Destroy(newBond.GetComponent(typeof(BondController)));
      newBond.AddComponent(typeof(FeedbackBondController));
      newBond.GetComponent<FeedbackBondController>().SetAtoms(atom1, atom2, bondNumber);
      newBond.transform.localScale += new Vector3(bondScale, bondScale, 0);
      lastInviBond = newBond;
      lastInviBondType = bondNumber;
    }

  }

  void CheckTranslate()
  {
    if (translate && canTranslate)
    {
      Vector3 v = handController.GetComponent<HandController>().GetHandMovement();
      transform.position -= v;
    }
  }

  void CheckRotate ()
  {
    for (int i = 0; i < transform.childCount; i++) {
      Transform child = transform.GetChild (i);
      if (child.CompareTag ("Interactable") && rotate) {
        if (rotationType == 1)
        {
          float a = 80 * Time.deltaTime;
          child.RotateAround(center, axis, a);
        }
        else if (rotationType == 2)
        {
          float rotation = (handController.GetComponent<HandController>().GetHandRotation()) * rotationOffset;
          if ((rotation > 0 && rotation < 1.3f) || (rotation < 0 && rotation > -1.3f))
            rotation = 0;
          child.RotateAround(center, axis, rotation);//forward
        }
        else if (rotationType == 3)
        {
          isRotating = true;
          //UpdateValueRotation();
          child.GetComponent<Atom>().SetRotating(true);
          
        }
      } else if (child.CompareTag ("Interactable") && !rotate) {
        if (rotationType == 1 || rotationType == 2)
          child.RotateAround (center, axis, 0);
        else {
          child.GetComponent<Atom> ().SetRotating (false);
          child.RotateAround(center, axis, 0);
          isRotating = false;
        }
      }
    }
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
        grabbedChild = child;
      }
      else if (child.CompareTag("Pivot") && child.GetComponent<InteractionBehaviour>().isGrasped)
        pivotGrabbed = true;
    }
    CheckMoleculeState();
  }

  void CheckMoleculeState()
  {
    if(rotationType == 3)
      rotate = false;
    canTranslate = false;
    
    //If 2 atoms are grasped and are not being bonded, you start detaching
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
    //If pivot and 1 atom are grasped it is to start rotating
    else if (pivotGrabbed && graspedAtoms >= 1 && rotationType == 3)
    {
      rotate = true;
      if(!isRotating)
      LockDistanceToCenter();
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

    if (pivotGrabbed && graspedAtoms == 0)
      canTranslate = true;

    if (rotate && canCenterInst)
    {
      centerRep = Instantiate(centerGO, center, transform.rotation);
      centerRep.transform.parent = transform;
      canCenterInst = false;
    }
    else if(!rotate)
    {
      canCenterInst = true;
      Destroy(centerRep);
    }

  }

  void LockDistanceToCenter()
  {
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Interactable"))
      {
        child.GetComponent<Atom>().SetDistanceToCenter(center);
        if (child != grabbedChild)
        {
          child.GetComponent<Atom>().SetDistanceToGrabbed(grabbedChild.position);
          child.GetComponent<Atom>().SetDistanceToOtherAtoms(grabbedChild.gameObject);
        }
      }
    }

    Vector3 dir = grabbedChild.position - center;
    //Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
    axis = Vector3.Cross(dir, Vector3.up).normalized;
  }

  private void UpdateValueRotation()
  {
    rotationValue = Vector3.Distance(lastPosGrabbedChild,grabbedChild.position);
    Vector3 dir = grabbedChild.position - center;
    axis = Vector3.Cross(dir, Vector3.up).normalized;
    lastPosGrabbedChild = grabbedChild.position;
  }

  public void SetAxis(bool endZGrabbed, bool endXGrabbed)
  {
    if (endZGrabbed)
      axis = Vector3.forward;
    if (endXGrabbed)
      axis = Vector3.right;
    if (!endZGrabbed && !endXGrabbed)
      axis = Vector3.zero;
  }

  public void AddBond(GameObject bond)
  {
    numberOfBonds++;
    bond.GetComponent<BondController>().SetID(numberOfBonds);
  }

  //Check what type of bond must be created
  void DefineBondType (GameObject atomA, GameObject atomB, int type)
  {
    bondType = type;
      
    switch (bondType) {
      case 1:
        bond=simpleBond;
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
  }

  private void DefineBondTypeTaps(GameObject atomA, GameObject atomB, int taps)
  {
    //get available bonds from the atoms
    int availableBondsA = atomA.GetComponent<Atom>().GetAvailableBonds();
    int availableBondsB = atomB.GetComponent<Atom>().GetAvailableBonds();
    if (availableBondsA <= 0 || availableBondsB <= 0)
      return;

    bondType = 0;
    //get the smallest bond type possible between the two atoms
    if (availableBondsA < availableBondsB)
      bondType = availableBondsA;
    else
      bondType = availableBondsB;

    if (taps <= bondType) bondType = taps;
    else {
      bondType = -1;
      return;
    }
    
    //define the gameobject of the bond to instantiate
    switch (bondType) {
      case 1:
        bond=simpleBond;
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
    
  }

  public void UpdateBondType(int value)
  {
    if (canUpdateTap)
    {
      numberOfTaps += value;
      if (numberOfTaps > 4)
        numberOfTaps = 1;
      canUpdateTap = false;
      Invoke("ResetTap",.3f);
    }
  }

  private void ResetTap()
  {
    canUpdateTap = true;
  }

  //Create a new bond between the given atoms
  public void CreateBond(GameObject atomA, GameObject atomB, bool loaded, int type)
  {
    //bonding automatically according to their available bonds
    if ((loaded))
    {
      DefineBondType(atomA, atomB, type);
      GameObject newBond = Instantiate(bond, transform.position, transform.rotation);
      AddBond(newBond);
      newBond.GetComponent<BondController>().SetAtoms(atomA, atomB, bondType);
      newBond.transform.localScale += new Vector3(bondScale, bondScale, 0);
      newBond.transform.parent = transform;
      atomA.transform.parent = transform;
      atomB.transform.parent = transform;
    }
    else //bonding according to distance and available bonds
    {
      atomA.GetComponent<Atom>().SettingBond();
      atomB.GetComponent<Atom>().SettingBond();
      bondingAtoms = true;
      atomA.transform.parent = transform;
      atomB.transform.parent = transform;
      atom1 = atomA;
      atom2 = atomB;
    }
  }

  private void CreateBondTaps (int taps)
  {
    DefineBondTypeTaps (atom1, atom2, taps);
    if (bondType != -1) {
      GameObject newBond = Instantiate (bond, transform.position, transform.rotation);
      AddBond (newBond);
      newBond.GetComponent<BondController> ().SetAtoms (atom1, atom2, bondType);
      newBond.transform.localScale += new Vector3 (bondScale, bondScale, 0);
      newBond.transform.parent = transform;
      atom1.transform.parent = transform;
      atom2.transform.parent = transform;
      bondingAtoms = false;
      lastInviBond.GetComponent<FeedbackBondController> ().DestroyBond (atom1, atom2);
    } else {
      lastInviBond.GetComponent<FeedbackBondController> ().DestroyBond (atom1, atom2);
    }
    bondType = 0;
    numberOfTaps = 1;
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

    if(lastChildCount != transform.childCount)
    {
      MM.UpdateMolecule(transform.gameObject);
      lastChildCount = transform.childCount;
    }

    //update pivot position according to the positions of the existent atoms in the molecule
    if (!isRotating && !canTranslate)
    {
      Vector3 molCenter = new Vector3(0, 0, 0);
      Vector3 lastPosition = new Vector3(0, 0, 0);
      int count = 0;
      float maxX = 0;
      for (int i = 0; i < transform.childCount; i++)
      {
        Transform child = transform.GetChild(i);
        if (child.CompareTag("Interactable"))
        {
          count++;
          molCenter += (child.transform.position);
          if (maxX < child.transform.position.x)
            maxX = child.transform.position.x;
        }
      }

      Vector3 newPos = molCenter / count;
      center = newPos;
      if (!rotate)
      {
        newPos.x += maxX + pivotOffset;
        pivot.transform.position = newPos;
      }
      if (bondingAtoms)
      {
        newPos.x += .2f;
        pivot.transform.position = newPos;
      }
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

  public Transform GetGrabbedAtom()
  {
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Interactable") && child.GetComponent<InteractionBehaviour>().isGrasped)
        return child;
    }
    return null;
  }
}
