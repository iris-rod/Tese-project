﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerButton : MonoBehaviour {
  private Animator animator;
  private bool m_Started;
  private MoleculeManager MM;

  public string Button;

  // Use this for initialization
  void Start()
  {
    m_Started = true;
    animator = GetComponent<Animator>();
    MM = GameObject.Find("GameManager").GetComponent<MoleculeManager>();
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
        if (colliders[i].transform.name.Split(' ')[0] == "Contact")
        {
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