using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButton : MonoBehaviour {

  private Animator animator;
  private bool m_Started;
  
  public string obj;

	// Use this for initialization
	void Start () {
    m_Started = true;
    animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
    CheckCollision();
	}

  private void CheckCollision ()
  {
    Collider[] colliders = Physics.OverlapBox (transform.position, transform.localScale / 5);
    if (colliders.Length > 1) {
      for (int i = 0; i < colliders.Length; i++) {
        if (colliders [i].transform.name.Split (' ') [0] == "Contact") {
          if (obj.ToLower () == "molecule") {
            GameObject[] molecules = GameObject.FindGameObjectsWithTag ("Molecule");
            for (int j = 0; j < molecules.Length; j++) {
              if (molecules [j].name.Split ('_') [0] != "Mini")
                Destroy (molecules [j]);
            }
          } else if (obj.ToLower () == "atom") {
            GameObject[] atoms = GameObject.FindGameObjectsWithTag ("Interactable");
            for (int j = 0; j < atoms.Length; j++) {
              if (atoms [j].transform.parent == null)
                Destroy (atoms [j]);
            }
          }
          animator.SetBool("pushed",true);
          Invoke("Reset", .5f);
          break;
        }
      }
    }
  }

  void Reset()
  {
    animator.SetBool("pushed", false);
  }
}
