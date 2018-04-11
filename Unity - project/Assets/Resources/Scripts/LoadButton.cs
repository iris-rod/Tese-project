using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour {

  public GameObject manager;
  
  private bool VR;
  private bool canLoad;
  private GameObject savedMolecule;
  private GameObject platform;
  GameObject inputField;  
  
  // Use this for initialization
  void Start()
  {
    canLoad = true;
    //inputField = transform.parent.GetComponent<InterfaceManager>().GetInputField();
    platform = GameObject.FindGameObjectWithTag("Platform");
    if(transform.parent.name == "Interface")
      VR = transform.parent.GetComponent<InterfaceManager>().camera.GetComponent<Manager>().VR;
    else if(transform.parent.name == "notepad")
      VR = transform.parent.GetComponent<NotepadController>().GetVR();
  }

  // Update is called once per frame
  void Update()
  {
    /*var se = new InputField.SubmitEvent();
    se.AddListener(SubmitName);
    inputField.GetComponent<InputField>().onEndEdit = se;*/
    CheckCollision();
  }

  private void SubmitName(string arg0)
  {
    manager.GetComponent<Manager>().LoadMolecule(arg0);
    inputField.SetActive(false);
  }

  void CheckCollision ()
  {
    Vector3 newScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z*3);
    Collider[] hitColliders = Physics.OverlapBox (transform.position, newScale);//Physics.OverlapSphere(transform.position, 0.012f);
    if (hitColliders.Length <= 2)
      canLoad = true;
  }
  

  void OnTriggerEnter (Collider col)
  {
    string[] name = col.transform.name.Split (' ');
    if (name [0] == "Contact" && !VR) {//name[0] == "Contact" && col.GetComponent<HandController>().IsPointing())
//      inputField.SetActive (true);
    } else if (name [0] == "Contact" && VR && canLoad) {
      if (savedMolecule != null && platform.GetComponent<Platform>().IsFree()){        
        transform.parent.GetComponent<InterfaceManager> ().Load (savedMolecule);
        canLoad = false;
      }
    }
  }
  
  public void SetSavedSelected (GameObject selected)
  {
    savedMolecule = selected;
  }
  
}
