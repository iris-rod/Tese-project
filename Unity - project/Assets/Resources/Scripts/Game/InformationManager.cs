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

  //multiple choice question
  private string correctAnswer;

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

  public string GetCorrectAnswer()
  {
    return correctAnswer;
  }

  //raw -> build-H2O ou place-H2O ou save-
  private string GetDisplayText(string raw) 
  {
    string[] split = raw.Split('-');
    string result = "";
    Debug.Log(raw);
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
      case "multiple choice":
        result += split[1] + "\n";
        correctAnswer = split[3];
        for(int i = 4; i < split.Length; i++)
        {
          string[] ans = split[i].Split('.'); 
          if(i == split.Length - 1)
          {
            string[] aux = ans[1].Split('\n');
            result += ans[0].ToUpper() + ": " + aux[0] + "\n";// a.2 -> A: 2

          }
          else
            result += ans[0].ToUpper() + ": " + ans[1] + "\n";// a.2 -> A: 2
        }
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
