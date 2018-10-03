using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerButton : MonoBehaviour {
  private Animator animator;

  private GameManager GM;
  private bool panelIsActive, canPush, canPlay;

  public string Button;

  // Use this for initialization
  void Start()
  {
    animator = GetComponent<Animator>();
    GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    panelIsActive = transform.parent.transform.parent.GetComponent<AnswerPanel>().IsPanelActive();
    canPush = true;
    canPlay = true;
  }

  // Update is called once per frame
  void Update()
  {
    panelIsActive = transform.parent.transform.parent.GetComponent<AnswerPanel>().IsPanelActive();
    CheckCollision();
  }

  private void CheckCollision()
  {
    Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 5);
    if (colliders.Length > 1)
    {
      for (int i = 0; i < colliders.Length; i++)
      {
        if (colliders[i].transform.name.Split(' ')[0] == "Contact" && panelIsActive && canPush)
        {
          canPush = false;
          SoundEffectsManager.PlaySound("buttonAnswer");
          GM.SetPressedAnswer(Button);
          GM.UpdateLevel();
          animator.SetBool("pushed", true);
          if (canPlay)
          {
            SoundEffectsManager.PlaySound("button");
            canPlay = false;
          }
          Invoke("Reset", 1f);
          break;
        }
      }
    }
  }

  void Reset()
  {
    canPush = true;
    canPlay = true;
    animator.SetBool("pushed", false);
  }
}
