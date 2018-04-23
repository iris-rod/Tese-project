using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour
{

  private int graspedAtoms;
  private int numberOfBonds;
  private bool pivotGrabbed;
  private bool canTranslate, rotate, translate;
  private GameObject bond;
  private float bondScale;
  private float pivotOffset;

  public GameObject simpleBond;
  public GameObject doubleBond;
  public GameObject tripleBond;
  public GameObject quadrupleBond;
  private GameObject lastInviBond;
  private int lastInviBondType;

  private Vector3 axis;

  public GameObject rotationToogle;
  private GameObject pivot;
  private GameObject handController;

  //variables picked up from the manager
  private int bondType;
  private int rotationType;
  private bool distanceToBond;
  private bool MultipleLines;
  private bool PointToLoad;

  //variables used to bond atoms
  private bool bondingAtoms;
  private GameObject atom1, atom2;
  private int distBond;

  //variables related to the mini molecules on the shelves
  private GameObject shelves;
  private bool isMini;
  private bool isPointed;
  private Vector3 hitScale;

  void Awake()
  {
    GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
    shelves = GameObject.Find("shelves");
    rotationType = camera.GetComponent<Manager>().rotationType;
    MultipleLines = camera.GetComponent<Manager>().MultipleLines;
    distanceToBond = camera.GetComponent<Manager>().DistanceToBond;
    PointToLoad = camera.GetComponent<Manager>().PointToLoad;
  }

  // Use this for initialization
  void Start()
  {
    pivotOffset = 0.1f;
    numberOfBonds = 1;
    graspedAtoms = 0;
    hitScale = new Vector3(0.03f, 0.1f, 0.1f);
    pivotGrabbed = false;
    rotate = false;
    isPointed = false;
    pivot = Instantiate(rotationToogle, transform.position, transform.rotation);
    pivot.transform.parent = transform;
    SetManagerPivot();
    UpdateStructure();
    isMini = false;
    string split = transform.name.Split('_')[0];
    if (split == "Mini")
    {
      isMini = true;
      pivotOffset = 0.04f;
    }
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
    if (!isMini)
    {
      CheckAtomsGrasped();
      UpdateStructure();
      graspedAtoms = 0;
      pivotGrabbed = false;

      Debug.Log(rotate);
      CheckRotate();
      CheckTranslate();
      if (bondingAtoms)
        CheckDistance();
    }
    else if (isMini)
    {
      CheckIfSelected();
    }
  }

  void CheckDistance()
  {
    float dist = Vector3.Distance(atom1.transform.position, atom2.transform.position);
    SetInvisibleBond(dist);
    if (!atom1.GetComponent<Atom>().IsBonding() && !atom2.GetComponent<Atom>().IsBonding())
      CreateBondDist(dist);
  }

  //creates the bond according to the distance to guide the user
  void SetInvisibleBond(float dist)
  {
    bond = simpleBond;
    int bondNumber = 1;
    if (dist <= .2f)
    {
      bond = quadrupleBond;
      bondNumber = 4;
    }
    else if (dist <= .3f && dist > .2)
    {
      bond = tripleBond;
      bondNumber = 3;
    }
    else if (dist <= .4f && dist > 0.3f)
    {
      bond = doubleBond;
      bondNumber = 2;
    }

    if (lastInviBondType != bondNumber)
    {
      if (lastInviBond != null)
        lastInviBond.GetComponent<FeedbackBondController>().DestroyBond(atom1, atom2);
      GameObject newBond = Instantiate(bond, transform.position, transform.rotation);
      Color color = newBond.GetComponent<MeshRenderer>().material.color;
      color.a = 0.4f;
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

  void CheckRotate()
  {
    Vector3 pointRotate = pivot.transform.position;
    pointRotate.y += pivotOffset;
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Interactable") && rotate)
      {
        if (rotationType == 1)
          child.RotateAround(pointRotate, axis, 80 * Time.deltaTime);//handController.GetComponent<HandController>().GetHandRotation()
        else if (rotationType == 2)
          child.RotateAround(pointRotate, Vector3.forward, handController.GetComponent<HandController>().GetHandRotation());
        else if (rotationType == 3)
          child.GetComponent<Atom>().SetRotating(true);
      }
      else if (child.CompareTag("Interactable") && !rotate)
      {
        if (rotationType == 1 || rotationType == 2)
          child.RotateAround(pointRotate, axis, 0);
        else
          child.GetComponent<Atom>().SetRotating(false);
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
        graspedAtoms++;
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
  void DefineBondType(GameObject atomA, GameObject atomB)
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
      if (bondType == 2)
        bondScale = 0.02f;
      else if (bondType == 3)
        bondScale = 0.03f;

    }
  }

  //Check what type of bond must be created
  void DefineBondTypeDist(GameObject atomA, GameObject atomB, float dist)
  {
    int distBond = 0;
    if (dist <= .2f)
      distBond = 4;
    else if (dist <= .3f && dist > .2)
      distBond = 3;
    else if (dist <= .4f && dist > 0.3f)
      distBond = 2;
    else if (dist > .4f)
      distBond = 1;

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

    if (distBond <= bondType) bondType = distBond;
    else return;

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
      if (bondType == 2)
        bondScale = 0.02f;
      else if (bondType == 3)
        bondScale = 0.03f;

    }
  }


  //Create a new bond between the given atoms
  public void CreateBond(GameObject atomA, GameObject atomB)
  {
    //bonding automatically according to their available bonds
    if (!distanceToBond)
    {
      DefineBondType(atomA, atomB);
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

  private void CreateBondDist(float dist)
  {
    DefineBondTypeDist(atom1, atom2, dist);
    GameObject newBond = Instantiate(bond, transform.position, transform.rotation);
    AddBond(newBond);
    newBond.GetComponent<BondController>().SetAtoms(atom1, atom2, bondType);
    newBond.transform.localScale += new Vector3(bondScale, bondScale, 0);
    newBond.transform.parent = transform;
    atom1.transform.parent = transform;
    atom2.transform.parent = transform;
    bondingAtoms = false;
    lastInviBond.GetComponent<FeedbackBondController>().DestroyBond(atom1, atom2);
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
    //update pivot position according to the positions of the existent atoms in the molecule
    if (!rotate)
    {
      Vector3 center = new Vector3(0, 0, 0);
      Vector3 lastPosition = new Vector3(0, 0, 0);
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

  //Check if the mini in the shelf was touched
  void CheckIfSelected()
  {
    Vector3 pos = new Vector3(transform.position.x, transform.position.y + .4f, transform.position.z + .03f);
    Collider[] hitColliders = Physics.OverlapBox(pos, hitScale);
    if (hitColliders.Length > 0)
    {

      for (int i = 0; i < hitColliders.Length; i++)
      {
        string name = hitColliders[i].name.Split(' ')[0];
        if (name == "Contact" && ((PointToLoad && isPointed)) || (!PointToLoad))
          shelves.GetComponent<ShelfManager>().LoadMolecule(transform.gameObject);
      }
    }
  }


  //highlight the mini on the shelf that is being pointed at
  public void HighlightMini(bool value)
  {
    for (int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Interactable"))
        child.GetComponent<Atom>().Highlight(value);
      else if (child.CompareTag("Bond"))
        child.GetComponent<BondController>().Highlight(value);
      else if (child.CompareTag("Pivot"))
        child.GetComponent<PivotController>().Highlight(value);
    }
    isPointed = value;
  }
}
