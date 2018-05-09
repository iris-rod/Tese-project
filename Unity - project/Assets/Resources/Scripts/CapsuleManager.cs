using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleManager : MonoBehaviour {

  private bool hasMolecule;
  private SaveButton save;
  
  void Start ()
  {
    hasMolecule = false;
  }

  void Update ()
  {
    CheckCollision();
  }

  void CheckCollision ()
  {
    bool foundMol = false;
    Collider[] colliders = Physics.OverlapBox (transform.position, transform.localScale / 2);
    if (colliders.Length > 1) {
      for (int i = 0; i < colliders.Length; i++) {
        if ((colliders [i].CompareTag ("Interactable") || colliders [i].CompareTag ("Pivot")) && !hasMolecule) {
          transform.GetChild (0).GetChild (0).GetComponent<SaveButton> ().SetAtom (colliders [i].transform.gameObject);
          hasMolecule = true;
          foundMol = true;
          break;
        }
      }
    }
    
    if (!foundMol) {
      hasMolecule = false;
      transform.GetChild (0).GetChild (0).GetComponent<SaveButton> ().SetAtom (null);
    }
    
  }
  
}
