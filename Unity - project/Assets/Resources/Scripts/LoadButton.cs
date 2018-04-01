﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour {

  public GameObject manager;

  GameObject inputField;
  // Use this for initialization
  void Start()
  {
    inputField = GameObject.FindGameObjectWithTag("FileNameInput");
    inputField.SetActive(false);
  }

  // Update is called once per frame
  void Update()
  {

    var se = new InputField.SubmitEvent();
    se.AddListener(SubmitName);
    inputField.GetComponent<InputField>().onEndEdit = se;
  }

  private void SubmitName(string arg0)
  {
    manager.GetComponent<Manager>().LoadMolecule(arg0);
    inputField.SetActive(false);
  }

  void OnTriggerEnter(Collider col)
  {
    string[] name = col.transform.name.Split(' ');
    if (col.CompareTag("Interactable"))//name[0] == "Contact" && col.GetComponent<HandController>().IsPointing())
    {
      //Manager.SaveMolecule(col.transform.parent.gameObject);
      inputField.SetActive(true);
    }
  }
}
