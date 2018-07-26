using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBoxRows : MonoBehaviour {

  private BoxAtomsManager BAM;
  private Quaternion initalRotation;
  private Vector3 initialPos;
  private Animator animator;
  private bool canPlay, canCheckCollision;

  // Use this for initialization
  void Start()
  {
    BAM = GameObject.Find("GameManager").GetComponent<BoxAtomsManager>();
    animator = GetComponent<Animator>();
    canPlay = true;
    canCheckCollision = true;
  }

  // Update is called once per frame
  void Update()
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
        if (colliders[i].transform.name.Split(' ')[0] == "Contact" && canCheckCollision)
        {
          BAM.ChangeRow();
          if (canPlay)
          {
            SoundEffectsManager.PlaySound("button");
            canPlay = false;
          }
          animator.SetBool("pushed", true);
          Invoke("Reset", .5f);
          canCheckCollision = false;
          break;
        }
      }
    }
  }

  void Reset()
  {
    animator.SetBool("pushed", false);
    canPlay = true;
    Invoke("ResetCollision", 0.5f);
  }

  void ResetCollision()
  {
    canCheckCollision = true;
  }

}
