using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAtoms : MonoBehaviour {

  public string atomType;
  public Leap.Unity.Interaction.InteractionManager manager;
  public GameObject camera;
  private GameObject handController;
  private Vector3 handPosition;
  private bool canPickNewAtom;
  private BoxCollider[] colliders;
  private Material atomMaterial;
  private int atomBondsAllowed;

  //testing variables
  private Vector3 spawnPoint;
  private bool leftPinched;
  private bool leftClosest;
  private bool spawnNewAtom;
  private bool TopOfBox;

  private bool resumeUpdate;
  public GameObject atom;

	// Use this for initialization
	void Start () {
    spawnPoint =  new Vector3(transform.position.x,transform.position.y + .1f,transform.position.z);//new Vector3(platformPosition.x, platformPosition.y + 0.1f, platformPosition.z);
    canPickNewAtom = true;
    resumeUpdate = true;
    spawnNewAtom = true;
    TopOfBox = true;
    handController = GameObject.FindGameObjectWithTag("HandController");
    colliders = GetComponents<BoxCollider>();
  }
	
	// Update is called once per frame
	void Update () {
    CheckManager();

    if (!TopOfBox)
    {
      if (resumeUpdate)
      {
          Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 0.01f);
          if (hitColliders.Length < 1)
            canPickNewAtom = true;
        
      }
    }
    else
    {
      spawnNewAtom = false;
      Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 0.01f);
      if (hitColliders.Length < 1)
        spawnNewAtom = true;
      if (spawnNewAtom)
      {
        GetMaterial();
        GameObject newAtom = Instantiate(atom, spawnPoint, Quaternion.identity);
        newAtom.GetComponent<Atom>().handController = handController;
        newAtom.GetComponent<Atom>().manager = manager;
        newAtom.GetComponent<Atom>().SetProperties(atomType, atomMaterial, atomBondsAllowed);
      }
    }
  }

  void CheckManager()
  {
    if (Input.GetKeyDown("j"))
    {
      TopOfBox = true;
    }
    if (Input.GetKeyDown("k"))
    {
      TopOfBox = false;
    }
  }

  void CheckCollider()
  {
    if (!canPickNewAtom)
    {
      colliders[1].enabled = true;
    }
    else
      colliders[1].enabled = false;
  }

  void Resume()
  {
    resumeUpdate = true;
  }

  void GetMaterial()
  {
    switch (atomType)
    {
      case "Oxygen":
        atomMaterial = Resources.Load("Materials/Oxygen 2", typeof(Material)) as Material;
        break;
      case "Hydrogen":
        atomMaterial = Resources.Load("Materials/Hydrogen 2", typeof(Material)) as Material;
        break;
      case "Carbon":
        atomMaterial = Resources.Load("Materials/Carbon 2", typeof(Material)) as Material;
        break;
      case "Nitrogen":
        atomMaterial = Resources.Load("Materials/Nitrogen 2", typeof(Material)) as Material;
        break;
    }
    atomBondsAllowed = Properties.BONDS[atomType];
  }

  bool IsHandPiched()
  {

    Hand left = handController.GetComponent<HandController>().GetLeftHand();
    Hand right = handController.GetComponent<HandController>().GetRightHand();

    GetClosestHand();
    if (leftClosest)
      return left.IsPinching();
    else
      return right.IsPinching();

  }

  void GetClosestHand()
  {
    Hand left = handController.GetComponent<HandController>().GetLeftHand();
    Hand right = handController.GetComponent<HandController>().GetRightHand();
    Vector3 leftPosition = Vector3.zero;
    Vector3 rightPosition = Vector3.zero;

    if (left != null && right != null)
    {
      leftPosition = new Vector3(left.PalmPosition.x, left.PalmPosition.y, left.PalmPosition.z);
      rightPosition = new Vector3(right.PalmPosition.x, right.PalmPosition.y, right.PalmPosition.z);

      if (Vector3.Distance(leftPosition, transform.position) < Vector3.Distance(rightPosition, transform.position))
        leftClosest = true;
      else
        leftClosest = false;
    }

    else if (left == null && right != null)
      leftClosest = false;
    else if (left != null && right == null)
      leftClosest = true;
  }

  void GetHandPosition()
  {
    Hand left = handController.GetComponent<HandController>().GetLeftHand();
    Hand right = handController.GetComponent<HandController>().GetRightHand();
    Vector3 leftPosition = Vector3.zero;
    Vector3 rightPosition = Vector3.zero;

    if (left != null && right != null)
    {
      leftPosition = new Vector3(left.PalmPosition.x, left.PalmPosition.y, left.PalmPosition.z);
      rightPosition = new Vector3(right.PalmPosition.x, right.PalmPosition.y, right.PalmPosition.z);

      if (Vector3.Distance(leftPosition, transform.position) < Vector3.Distance(rightPosition, transform.position))
        handPosition = leftPosition;
      else
        handPosition = rightPosition;
    }

    else if (left == null && right != null)
    {
      rightPosition = new Vector3(right.PalmPosition.x, right.PalmPosition.y, right.PalmPosition.z);
      handPosition = rightPosition;
    }
    else if (left != null && right == null)
    {
      leftPosition = new Vector3(left.PalmPosition.x, left.PalmPosition.y, left.PalmPosition.z);
      handPosition = leftPosition;
    }
  }

}
