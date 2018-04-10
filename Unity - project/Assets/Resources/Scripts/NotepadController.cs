using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class NotepadController : MonoBehaviour {

  GameObject camera;
  GameObject inputField;
  Animator animator;
  private bool VR;
  private bool picked;

  void Awake(){
    VR = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Manager>().VR;
  }  

  // Use this for initialization
  void Start()
  {
    picked = false;
    animator = GetComponent<Animator>();
    camera = GameObject.FindGameObjectWithTag("MainCamera");
    inputField = GameObject.FindGameObjectWithTag("FileNameInput");
    inputField.SetActive(false);
  }

  void Update()
  {
    if(GetComponent<InteractionBehaviour>().isGrasped) picked = true;
  }

  public void Open()
  {
    animator.SetTrigger("open");
    Invoke("Close",3);
  }

  public void Close()
  {
    animator.SetTrigger("close");
  }

  public bool GetVR ()
  {
    return VR;
  }

  public bool IsPicked ()
  {
    return picked;
  }

  public void Save(GameObject mol, string name)
  {
    camera.GetComponent<Manager>().SaveMolecule(mol, name);
  }

  public void Load(GameObject mol)
  {
    camera.GetComponent<Manager>().LoadMolecule(mol.GetComponent<SavedMolecule>().GetFileName());
  }
}
