using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedButton : MonoBehaviour {

  private Animator animator;
  private GameObject molecule;
  private GameObject capsule;
  
  // Use this for initialization
  void Start () {
    animator = GetComponent<Animator>();
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
        if (colliders [i].transform.name.Split (' ') [0] == "Contact") {
          GameObject invi = GameObject.FindGameObjectWithTag ("Invisible");
          if (invi.GetComponent<InvisibleMoleculeBehaviour> ().HasOverlap ()) {
            Debug.Log("has overlap");
            animator.SetBool ("pushed", true);
            invi.GetComponent<InvisibleMoleculeBehaviour>().DestroyOverlap();
            Destroy(invi);          
            Invoke ("Reset", .5f);  
          }
        }
      }
    }
  }
  

  void Reset()
  {
    animator.SetBool("pushed", false);
  }
  
}
