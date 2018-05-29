using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadButton : MonoBehaviour {

  private Animator animator;
  private GameObject molecule;
  private GameObject capsule;

  private bool levelChecking;
  private GameManager GM;

  // Use this for initialization
  void Start () {
    levelChecking = false;
    animator = GetComponent<Animator>();
    capsule = GameObject.Find("LoadCapsule");
    GM = GameObject.Find("GameManager").GetComponent<GameManager>();
  }
  
  // Update is called once per frame
  void Update () {
    CheckCollision();
  }

  private void CheckCollision ()
  {
    Collider[] colliders = Physics.OverlapBox (transform.position, transform.localScale / 10);
    if (colliders.Length > 1) {
      for (int i = 0; i < colliders.Length; i++) {
        if (colliders [i].transform.name.Split (' ') [0] == "Contact" && molecule != null) {
          GameObject newMol = transform.parent.transform.parent.GetComponent<ShelfManager> ().LoadMolecule (molecule);
          if (newMol != null) {
            animator.SetBool ("pushed", true);
            capsule.GetComponent<LoadCapsule> ().OpenCapsule (newMol);
            Invoke ("Reset", .5f);
          }
          if (levelChecking)
            GM.SetLoadedMolecule(newMol);
          break;
        }
      }
    }
  }

  void Reset()
  {
    animator.SetBool("pushed", false);
  }
  
  public void SetMolecule (GameObject mol)
  {
    molecule = mol;
  }
  
  public bool HasMolecule ()
  {
    return (molecule != null);
  }

  public void LevelCheck(bool value)
  {
    levelChecking = value;
  }

}
