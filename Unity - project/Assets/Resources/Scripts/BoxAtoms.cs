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
  private Settings Settings;
  private GameManager GM;

  //animator
  private Animator animator;
  private bool moving,isUp;

  //testing variables
  private Vector3 spawnPoint;
  private bool leftPinched;
  private bool leftClosest;
  private bool spawnNewAtom;

  private bool resumeUpdate;
  public GameObject atom;

	// Use this for initialization
	void Start () {
    spawnPoint =  new Vector3(transform.position.x,transform.position.y + .1f,transform.position.z);//new Vector3(platformPosition.x, platformPosition.y + 0.1f, platformPosition.z);
    canPickNewAtom = true;
    resumeUpdate = true;
    spawnNewAtom = true;
    moving = false;
    isUp = true;
    handController = GameObject.FindGameObjectWithTag("HandController");
    colliders = GetComponents<BoxCollider>();
    Settings = GameObject.Find("GameManager").GetComponent<Settings>();
    animator = transform.parent.GetComponent<Animator>();
    GM = GameObject.Find("GameManager").GetComponent<GameManager>();
  }
	
	// Update is called once per frame
	void Update () {


      spawnNewAtom = false;
      Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 0.01f);
      if (hitColliders.Length < 1 && !moving && isUp)
        spawnNewAtom = true;
      if (spawnNewAtom)
      {
        GetMaterial();
        GameObject newAtom = Instantiate(atom, spawnPoint, Quaternion.identity);
        newAtom.GetComponent<Atom>().handController = handController;
        newAtom.GetComponent<Atom>().manager = manager;
        newAtom.GetComponent<Atom>().SetProperties(atomType, atomMaterial, atomBondsAllowed);
        GM.UpdatePointSystem();
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
      case "Oxigen":
        atomMaterial = Resources.Load("Materials/Oxigen 2", typeof(Material)) as Material;
        atomMaterial.mainTexture = Settings.GetAtomTexture(atomType);
        break;
      case "Hidrogen":
        atomMaterial = Resources.Load("Materials/Hidrogen 2", typeof(Material)) as Material;
        atomMaterial.mainTexture = Settings.GetAtomTexture(atomType);
        break;
      case "Carbon":
        atomMaterial = Resources.Load("Materials/Carbon 2", typeof(Material)) as Material;
        atomMaterial.mainTexture = Settings.GetAtomTexture(atomType);
        break;
      case "Nitrogen":
        atomMaterial = Resources.Load("Materials/Nitrogen 2", typeof(Material)) as Material;
        atomMaterial.mainTexture = Settings.GetAtomTexture(atomType);
        break;
      case "Fluorine":
        atomMaterial = Resources.Load("Materials/Fluorine 2", typeof(Material)) as Material;
        atomMaterial.mainTexture = Settings.GetAtomTexture(atomType);
        break;
      case "Chlorine":
        atomMaterial = Resources.Load("Materials/Chlorine 2", typeof(Material)) as Material;
        atomMaterial.mainTexture = Settings.GetAtomTexture(atomType);
        break;
      case "Bromine":
        atomMaterial = Resources.Load("Materials/Bromine 2", typeof(Material)) as Material;
        atomMaterial.mainTexture = Settings.GetAtomTexture(atomType);
        break;
      case "Iodine":
        atomMaterial = Resources.Load("Materials/Iodine 2", typeof(Material)) as Material;
        atomMaterial.mainTexture = Settings.GetAtomTexture(atomType);
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

  public void Move(bool dir)
  {
    isUp = !isUp;
    animator.SetBool("move", true);
    Invoke("Reset", .5f);
    moving = true;
    UpdateAtom();
  }

  private void UpdateAtom()
  {
    Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 0.01f);
    if (hitColliders.Length > 0)
    {
      Destroy(hitColliders[0].gameObject);
    }
    
  }

  void Reset()
  {
    animator.SetBool("move", false);
    Invoke("AnimationEnd", 0.5f);
  }

  // prevents from atoms to appear before the animation ends
  void AnimationEnd()
  {
    moving = false;
  }

}
