using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerPanel : MonoBehaviour {
  private Animator animator;
  private bool isUp;
  public bool Multiple;


  // Use this for initialization
  void Start () {
    GameObject.Find("GameManager").GetComponent<GameManager>().SetPanelAnswer(transform.gameObject, Multiple);
    animator = GetComponent<Animator>();
  }
	
  public void Appear()
  {
    isUp = true;
    animator.SetBool("push", true);
  }

  public void Disappear()
  {
    Invoke("Down",1f);
  }

  public bool IsPanelActive()
  {
    return isUp;
  }

  private void Down()
  {
    isUp = false;
    animator.SetBool("push", false);
  }
}
