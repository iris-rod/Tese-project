using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBox : MonoBehaviour {

  public string type;
  public string levels;
  public Leap.Unity.Interaction.InteractionManager manager;
  public GameObject Ball;

  private GameObject handController;
  private Vector3 handPosition;
  private bool canPickNewAtom;
  private BoxCollider[] colliders;
  private Material atomMaterial;
  private int atomBondsAllowed;


  //testing variables
  private Vector3 spawnPoint;
  private bool spawnNewAtom;
  

  // Use this for initialization
  void Start()
  {
    spawnPoint = new Vector3(transform.position.x, transform.position.y + .1f, transform.position.z);//new Vector3(platformPosition.x, platformPosition.y + 0.1f, platformPosition.z);
    canPickNewAtom = true;
    spawnNewAtom = true;
    handController = GameObject.FindGameObjectWithTag("HandController");
    colliders = GetComponents<BoxCollider>();
  }

  // Update is called once per frame
  void Update()
  {
    spawnNewAtom = false;
    Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 0.01f);
    if (hitColliders.Length < 1)
      spawnNewAtom = true;
    if (spawnNewAtom)
    {
      GetMaterial();
      GameObject newBall = Instantiate(Ball, spawnPoint, Quaternion.identity);
      newBall.GetComponent<MainMenuBall>().Settings(handController, manager, type, levels);
      newBall.GetComponent<MeshRenderer>().material = atomMaterial;
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

  void GetMaterial()
  {
    switch (type)
    {
      case "Normal":
        atomMaterial = Resources.Load("Materials/Boxes/Nitrogen", typeof(Material)) as Material;
        break;
      case "SpeedRun":
        atomMaterial = Resources.Load("Materials/Boxes/Oxygen", typeof(Material)) as Material;
        break;
      case "MultipleChoice":
        atomMaterial = Resources.Load("Materials/Boxes/Fluorine", typeof(Material)) as Material;
        break;
      case "Complete":
        atomMaterial = Resources.Load("Materials/Boxes/Bromine", typeof(Material)) as Material;
        break;
      case "Transform":
        atomMaterial = Resources.Load("Materials/Boxes/Iodine", typeof(Material)) as Material;
        break;
      case "Tutorial":
        atomMaterial = Resources.Load("Materials/Boxes/Hydrogen", typeof(Material)) as Material;
        break;
      case "Build":
        atomMaterial = Resources.Load("Materials/Boxes/Carbon", typeof(Material)) as Material;
        break;
    }
  }

}
