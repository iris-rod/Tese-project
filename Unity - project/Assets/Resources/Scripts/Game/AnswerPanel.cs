using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerPanel : MonoBehaviour {
  private Animator animator;
  public string Type;

  // Use this for initialization
  void Start () {

    animator = GetComponent<Animator>();
  }
	
	// Update is called once per frame
	void Update () {
  }

  public void Appear()
  {
    animator.SetBool("push", true);
  }

  public void Disappear()
  {
    Invoke("Down",1f);
  }

  private void Down()
  {
    animator.SetBool("push", false);
  }
}
