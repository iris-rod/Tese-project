using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedButton : MonoBehaviour {

  private Animator animator;
  private GameObject molecule;
  private GameObject capsule;
  private TestsManager TM;
  
  // Use this for initialization
  void Start () {
    animator = GetComponent<Animator>();
    TM = Camera.main.GetComponent<TestsManager>();
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
          if (invi != null && invi.GetComponent<InvisibleMoleculeBehaviour> ().HasOverlap ()) {
            animator.SetBool ("pushed", true);
            invi.GetComponent<InvisibleMoleculeBehaviour>().DestroyOverlap();
            Destroy(invi);          
            //LogsC.Instance.sessionStopSubTask();
            //TM.StopSubTask();
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
