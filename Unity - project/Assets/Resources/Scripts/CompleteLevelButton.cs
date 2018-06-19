using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteLevelButton : MonoBehaviour {

  private GameManager GM;
  private Animator animator;
  private bool canPush;

  // Use this for initialization
  void Start () {
    GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    animator = GetComponent<Animator>();
    canPush=true;
  }
	
	// Update is called once per frame
	void Update () {
    CheckCollision();
  }

  private void CheckCollision()
  {
    Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 10);
    if (colliders.Length > 1)
    {
      for (int i = 0; i < colliders.Length; i++)
      {
        if (colliders[i].transform.name.Split(' ')[0] == "Contact" && canPush)
        {
          //canPush = false;
          //animator.SetBool("pushed", true);
          //Invoke("Reset", .5f);
          //GM.UpdateLevel();
          GameObject invi = GameObject.FindGameObjectWithTag("Invisible");
          if (invi != null && invi.GetComponent<InvisibleMoleculeBehaviour>().HasOverlap())
          {
            canPush = false;
            animator.SetBool("pushed", true);
            invi.GetComponent<InvisibleMoleculeBehaviour>().DestroyOverlap();
            Destroy(invi);
            Invoke("Reset", .5f);
          }

        }
      }
    }
  }
  
    void Reset()
  {
          canPush = true;
    animator.SetBool("pushed", false);
  }

}
