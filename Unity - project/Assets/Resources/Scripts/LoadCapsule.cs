using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCapsule : MonoBehaviour {

  private Animator animator;
  private bool hasMolecule;
  private GameObject molecule;

  // Use this for initialization
  void Start () {
    hasMolecule = false;
    animator = GetComponent<Animator>();
  }
  
  // Update is called once per frame
  void Update ()
  {
    if (hasMolecule) {
      CheckCollision ();
      if(molecule != null)
        UpdateMolecule();
    }
      
  }
  
  private void UpdateMolecule ()
  {
    Vector3 molPos = new Vector3(transform.position.x+.65f, molecule.transform.position.y, molecule.transform.position.z);
    molecule.transform.position = molPos;
  }

  private void CheckCollision ()
  {
    Collider[] colliders = Physics.OverlapBox (transform.position, transform.localScale / 10);
    bool hasMol = false;
    if (colliders.Length > 1) {
      for (int i = 0; i < colliders.Length; i++) {
        if (colliders [i].CompareTag ("Interactable")) {
          hasMol = true;
        }
      }
    }
    
    if(!hasMol)
      CloseCapsule();
  }
  
  public void CloseCapsule ()
  {

    animator.SetBool("close", true);
    hasMolecule = false;
    molecule = null;
    Invoke("Reset", .5f);
  }
  
  public void OpenCapsule (GameObject mol)
  {
    animator.SetBool("open", true);
    hasMolecule = true;
    molecule = mol;
    Invoke("Reset", 0.5f);
  }

  void Reset()
  {
    animator.SetBool("close", false);
    animator.SetBool("open", false);
  }
  
  
}
