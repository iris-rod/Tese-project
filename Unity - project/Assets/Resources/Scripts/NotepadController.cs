using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotepadController : MonoBehaviour {

  GameObject camera;
  GameObject inputField;
  Animator animator;
  

  // Use this for initialization
  void Start()
  {
    animator = GetComponent<Animator>();
    camera = GameObject.FindGameObjectWithTag("MainCamera");
    inputField = GameObject.FindGameObjectWithTag("FileNameInput");
    inputField.SetActive(false);
  }

  void Update()
  {
    
  }

  public void Open()
  {
    animator.SetTrigger("open");
  }

  public void Close()
  {
    animator.SetTrigger("close");
  }


  public GameObject GetInputField()
  {
    return inputField;
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
