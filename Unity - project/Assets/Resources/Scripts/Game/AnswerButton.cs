using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerButton : MonoBehaviour {
  private Animator animator;

  private GameManager GM;
  private bool panelIsActive;

  public string Button;

  // Use this for initialization
  void Start()
  {
    animator = GetComponent<Animator>();
    GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    panelIsActive = transform.parent.transform.parent.GetComponent<AnswerPanel>().IsPanelActive();
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
        if (colliders[i].transform.name.Split(' ')[0] == "Contact" && panelIsActive)
        {
          SoundEffectsManager.PlaySound("buttonAnswer");
          GM.SetPressedAnswer(Button);
          GM.UpdateLevel();
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
  }
}
