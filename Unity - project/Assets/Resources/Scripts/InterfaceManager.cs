using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour {

  public bool VR;
  GameObject inputField;

	// Use this for initialization
	void Start ()
  {
    inputField = GameObject.FindGameObjectWithTag ("FileNameInput");
    inputField.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
  
  public GameObject GetInputField ()
  {
    return inputField;
  }
}
