using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour {
  
  private bool VR;
  GameObject inputField;
  GameObject molecule;
  int moleculeID;

	// Use this for initialization
	void Start () {
    inputField = transform.parent.GetComponent<InterfaceManager>().GetInputField();
    VR = transform.parent.GetComponent<InterfaceManager>().VR;
    if(VR){
      moleculeID = 1;
    }
	}
	
	// Update is called once per frame
	void Update () {

    var se = new InputField.SubmitEvent();
    se.AddListener(SubmitName);
    inputField.GetComponent<InputField>().onEndEdit = se;
  }

  private void SubmitName(string arg0)
  {
    transform.parent.GetComponent<InterfaceManager>().Save(molecule,arg0);
    inputField.SetActive(false);
  }

  void OnTriggerEnter (Collider col)
  {
    string[] name = col.transform.name.Split (' ');
    if (col.CompareTag ("Interactable") && !VR) {//name[0] == "Contact" && col.GetComponent<HandController>().IsPointing())
      inputField.SetActive (true);
      molecule = col.gameObject;
    } 
    else if (col.CompareTag ("Interactable") && VR) {
      transform.parent.GetComponent<InterfaceManager>().Save(col.transform.parent.gameObject,"saved_" + moleculeID.ToString());
      moleculeID++;
    }
    Destroy(col.transform.parent.transform.gameObject);
  }
}
