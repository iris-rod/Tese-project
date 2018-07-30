using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour {

  private TextMeshPro logText;
  private int logIndex;

  // Use this for initialization
  void Start ()
  {
    logIndex = 1;
    logText = transform.GetChild(0).GetComponent<TextMeshPro>();
    GameObject.Find("GameManager").GetComponent<MoleculeManager>().SetUp();
  }

  public void AddLog(string newEntry)
  {
    logText.text += logIndex.ToString() +"- " + newEntry + "\n";
    logIndex++;
  }


}
