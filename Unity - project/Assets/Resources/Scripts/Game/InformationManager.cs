using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationManager : MonoBehaviour {

  private TextMeshPro levelText;
  private TextMeshPro objectivesText;
  private GameObject check;
  private string currentDisplay;
  private string updatedDisplay;
	// Use this for initialization
	void Start () {
    levelText = transform.GetChild(0).GetComponent<TextMeshPro>();
    objectivesText = transform.GetChild(1).GetComponent<TextMeshPro>();
    check = transform.GetChild(2).gameObject;
    check.SetActive(false);
	}
	
  public void UpdateDisplay(string newObj, bool newLevel)
  {
    if (!newLevel)
    {
      check.SetActive(true);
      updatedDisplay = GetDisplayText(newObj);
      Debug.Log(updatedDisplay);
      Invoke("NewDisplay", 1f);
    }
    else
    {
      objectivesText.text = GetDisplayText(newObj);
    }
  }

  public void UpdateLevel(int level)
  {
    levelText.text = "LEVEL " + level;
  }

  //raw -> build-H2O ou place-H2O ou save-
  private string GetDisplayText(string raw) 
  {
    string[] split = raw.Split('-');
    string result = "";
    switch (split[0])
    {
      case "build":
        result += "Build a " + split[1] + " molecule.";
        break;
      case "place":
        result += "Move the " + split[1] + " molecule to overlap the transparent molecule.";
        break;
      case "save":
        result += "Put the " + split[1] + " molecule in the shelf.";
        break;
      case "load":
        result += "Get the " + split[1] + " molecule from the shelf.";
        break;
    }
    currentDisplay = result;
    return result;
  }

  void NewDisplay()
  {
    check.SetActive(false);
    objectivesText.text = updatedDisplay;
  }

}
