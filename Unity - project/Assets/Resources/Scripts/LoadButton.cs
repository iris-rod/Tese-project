using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour {

  public GameObject manager;
  private bool VR;
  
  GameObject inputField;
  
  // Use this for initialization
  void Start()
  {
    inputField = transform.parent.GetComponent<InterfaceManager>().GetInputField();
    VR = transform.parent.GetComponent<InterfaceManager>().VR;
  }

  // Update is called once per frame
  void Update()
  {
    var se = new InputField.SubmitEvent();
    se.AddListener(SubmitName);
    inputField.GetComponent<InputField>().onEndEdit = se;
  }

  private void SubmitName(string arg0)
  {
    manager.GetComponent<Manager>().LoadMolecule(arg0);
    inputField.SetActive(false);
  }

  void OnTriggerEnter(Collider col)
  {
    string[] name = col.transform.name.Split(' ');
    if (col.CompareTag("Contact") && !VR)//name[0] == "Contact" && col.GetComponent<HandController>().IsPointing())
    {
      //Manager.SaveMolecule(col.transform.parent.gameObject);
      inputField.SetActive(true);
    }
    else if(col.CompareTag("Contact") && VR){
    
    }
  }
}
