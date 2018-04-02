﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedMolecule : MonoBehaviour {
  
  private Material highlight;  
  private string fileName;
  private bool selected;
  private bool canSelect;
  private bool oneSelected;
  
  
  private bool TTS;
  
  void Start ()
  {
    selected = false;    
    oneSelected = false;
    highlight = transform.GetComponent<MeshRenderer>().materials[1];
    TTS = transform.parent.GetComponent<Manager>().touchOtherToSwitch;
  }
  
  public void SetFileName (string name)
  {
    fileName = name;
  }
  
  public string GetFileName ()
  {
    return fileName;
  }
  
  void Update ()
  {   
    if (!TTS) { //testing -> select A, to select B, must deselect A first
      if (!oneSelected)
        CheckCollision ();
      else
        canSelect = false;  
    }
    else { // testing -> select A, to select B just touch it
      CheckCollision ();
    }
  }
  
  void CheckCollision ()
  {
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.012f);
    if (hitColliders.Length <= 1)
      canSelect = true;
  }
   
  void OnTriggerStay (Collider col) //ontriggerEnter
  {
    string[] name = col.transform.name.Split (' ');
    if (canSelect) {
      if (name [0] == "Contact" && !selected) {
        selected = true;
        highlight.SetFloat("_Outline", 0.015f);
      }
      else if(name [0] == "Contact" && selected){
        selected = false;        
        highlight.SetFloat("_Outline", 0);
      }
      canSelect = false;
    }
  }
  
  public void SetOneSelected (bool value)
  {
    oneSelected = value;
  }
  
  public bool IsSelected ()
  {
    return selected;
  }
 
}
