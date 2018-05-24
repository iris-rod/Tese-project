using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour {

  private bool VR;
  private int moleculeID;
  private bool canSave;
  private GameObject platform;
  private bool start;
 
  private GameObject mini;
  private bool newMini;
  private string lastPointedMini;
  private bool waiting;
  
  private GameObject[] spots;

  // Use this for initialization
  void Start ()
  {
    start = true;
    platform = GameObject.Find ("InviPlatform");
    moleculeID = 1;
    newMini = true;
    canSave = true;
    waiting = false;
    lastPointedMini = "";
    spots = GameObject.FindGameObjectsWithTag ("Interface");
    int id = 0;
    for (int i = 0; i < transform.childCount; i++) {
      Transform child  = transform.GetChild(i);
      if (child.name.Split (' ') [0] == "Button") {
        spots [id] = child.GetChild (0).gameObject;
        id++;
      }
    }
	}
	
	// Update is called once per frame
	void Update ()
  {
    if (start)
    {
      VR = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Manager>().VR;
      start = false;
    }
    if (mini != null && newMini) {
      Vector3 pos = GetMiniPosition ();
      
      //desktop version values for the mini mols on the shelves
      if (!VR) {
        mini.transform.localScale -= new Vector3 (0.8f, 0.8f, 0.8f);
      }
      //VR version values for the mini mols on the shelves
      else {
        mini.transform.localScale -= new Vector3 (0.2f, 0.2f, 0.2f);
      }

      mini.transform.parent = transform;
      mini.transform.position = pos;
      newMini = false;
    }
  }

  private Vector3 GetMiniPosition ()
  {
    Transform button =  SetMoleculeOnSpot();
    Vector3 spotPosition = new Vector3(button.parent.position.x, button.parent.position.y, button.parent.position.z);
    //change atoms position closer to the molecule object
    for(int i = 0; i < mini.transform.childCount; i++)
    {
      Transform child = mini.transform.GetChild(i);
      Vector3 pos = new Vector3(child.position.x + .5f, child.position.y-1.8f, child.position.z+.2f);
      child.position = pos;
    }
    return spotPosition;
  }
  
  private Transform SetMoleculeOnSpot ()
  {
    for (int i = 0; i < spots.Length; i++) {
      if (!spots [i].GetComponent<LoadButton> ().HasMolecule()) {
        spots [i].GetComponent<LoadButton> ().SetMolecule (mini);
        return spots[i].transform;
      }
    }
    return null;
  }

  public void SaveMolecule(GameObject mol)
  {
    if (canSave)//col.CompareTag("Interactable") && canSave)
    {
      GetComponent<InterfaceManager>().Save(mol.transform.parent.gameObject, "saved_" + moleculeID.ToString());//
      CreateMiniMolecule(mol.transform.parent);//
      canSave = false;
      Invoke("ResetSave",1f);
      moleculeID++;
    }
  }


  void ResetSave()
  {
    canSave = true;
  }

  private void CreateMiniMolecule(Transform molecule)
  {
    GetComponent<InterfaceManager>().Load(true,"saved_" + moleculeID);
    mini = GameObject.Find("Mini_saved_"+(moleculeID).ToString());
    //mini.gameObject.AddComponent(typeof(BoxCollider));
    newMini = true;
  }

  public GameObject LoadMolecule(GameObject molecule)
  {
    if (!waiting && true)//platform.GetComponent<Platform>().IsFree())
    {
      string name = molecule.name.Split('_')[1] + "_" + molecule.name.Split('_')[2];
      GameObject newMol = GetComponent<InterfaceManager>().Load(false, name);
      waiting = true;
      Invoke("Wait",1);
      return newMol;
    }
    return null;
  }

  void Wait()
  {
    waiting = false;
  }
  
  //return a list of gameobjects of saved molecules
  public void GetSavedMolecules()
  {

  }

  
}
