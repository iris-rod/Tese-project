using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour {
  
  public GameObject camera;
  private GameObject inputField;

	// Use this for initialization
	void Start ()
  {
    /*inputField = GameObject.FindGameObjectWithTag ("FileNameInput");
    inputField.SetActive (false);*/
	}

  public GameObject GetInputField ()
  {
    return inputField;
  }
  
  public void Save (GameObject mol, string name)
  {
    camera.GetComponent<Manager>().SaveMolecule(mol,name);
  }

  public void Load (GameObject mol)
  {
    camera.GetComponent<Manager>().LoadMolecule(mol.GetComponent<SavedMolecule>().GetFileName());
  }
}
