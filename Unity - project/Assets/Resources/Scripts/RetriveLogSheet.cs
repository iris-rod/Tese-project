using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetriveLogSheet : MonoBehaviour {

  private GameObject logSheet;
  private Quaternion initalRotation;
  private Vector3 initialPos;
  private Animator animator;
  private bool canPlay;
  
  // Use this for initialization
  void Start () {
    logSheet = GameObject.Find("LogSheet");
    initialPos = logSheet.transform.position;
    initalRotation = logSheet.transform.rotation;
    animator = GetComponent<Animator>();
    canPlay = true;
  }
	
	// Update is called once per frame
	void Update ()
  {
    CheckCollision();

  }

  private void CheckCollision()
  {
    Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 5);
    if (colliders.Length > 1)
    {
      for (int i = 0; i < colliders.Length; i++)
      {
        if (colliders[i].transform.name.Split(' ')[0] == "Contact")
        {
          logSheet.transform.position = initialPos;
          logSheet.transform.rotation = initalRotation;
          if (canPlay)
          {
            SoundEffectsManager.PlaySound("button");
            canPlay = false;
          }
          animator.SetBool("pushed", true);
          Invoke("Reset", .5f);
          break;
        }
      }
    }
  }

  void Reset()
  {
    animator.SetBool("pushed", false);
    canPlay = true;
  }

}
