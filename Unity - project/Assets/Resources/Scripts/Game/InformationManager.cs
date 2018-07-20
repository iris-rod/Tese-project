using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationManager : MonoBehaviour {

  private TextMeshPro levelText;
  private TextMeshPro objectivesText;
  private TextMeshPro pointsText;
  private GameObject check;
  private string currentDisplay;
  private string updatedDisplay;

  //multiple choice question
  private string correctAnswer;

	// Use this for initialization
	void Start () {
    levelText = transform.GetChild(0).GetComponent<TextMeshPro>();
    pointsText = transform.GetChild(1).GetComponent<TextMeshPro>();
    objectivesText = transform.GetChild(2).GetComponent<TextMeshPro>();
    check = transform.GetChild(3).gameObject;
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

  public void UpdateTimer(float min, float sec)
  {
    pointsText.text = "Timer: " + min + ":" + sec;
  }

  public void UpdateMoves(int moves)
  {
    pointsText.text = "Moves: " + moves;
  }

  //raw -> build-H2O ou place-H2O ou save-
  private string GetDisplayText(string raw) 
  {
    string[] split = raw.Split('-');
    string result = "";
    switch (split[0])
    {
      case "build":
        string moleculeDisp = CheckMoleculeRepresentation(split[1]);
        result += "Cria uma molécula \n" + moleculeDisp;
        break;
      case "complete":
        string moleculeDescr = CheckMoleculeDescription(split[1]);
        result += "Completa a molécula " + moleculeDescr;
        break;
      case "transform":
        result += "Transforma a molécula em " + split[1];
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
        string tempResult = "";
        for(int i = 4; i < split.Length; i++)
        {
          string[] ans = split[i].Split('.');
          if (i == split.Length - 1)
          {
            string[] aux = ans[1].Split('\n');
            tempResult = ans[0].ToUpper() + ": " + aux[0] + "\n";// a.2 -> A: 2
            result += tempResult;
          }
          else
          {
            tempResult = ans[0].ToUpper() + ": " + ans[1] + "\n";// a.2 -> A: 2   
            result += tempResult;
          }
        }
        break;
    }
    currentDisplay = GetSpecialCharacters(result);
    return currentDisplay;
  }

  private string GetSpecialCharacters(string raw)
  {
    if (raw.Length != 0)
    {
      string[] a = new string[] { "Ž"  , "ž"  , "Ÿ"  , "¡"  , "¢"  , "£"  , "¤"  , "¥"  , "¦"  , "§"  , "¨"  , "©"  , "ª"  , "À"  , "Á"  , "Â"  , "Ã"  , "Ä"  , "Å"  , "Æ"  , "Ç"  , "È"  , "É"  , "Ê"  , "Ë"  , "Ì"  , "Í"  , "Î"  , "Ï"  , "Ð"  , "Ñ"  , "Ò"  , "Ó"  , "Ô"  , "Õ"  , "Ö"  , "Ù"  , "Ú"  , "Û"  , "Ü"  , "Ý"  , "Þ"  , "ß"  , "à"          , "á"          , "â"  , "ã"          , "ä"  , "å"  , "æ"  , "ç"  , "è"  , "é"          , "ê"  , "ë"  , "ì"  , "í"  , "î"  , "ï"  , "ð"  , "ñ"  , "ò"  , "ó"  , "ô"  , "õ"  , "ö"  , "ù"  , "ú"  , "û"  , "ü"  , "ý"  , "þ"  , "ÿ" };
      string[] b = new string[] { "%8E", "%9E", "%9F", "%A1", "%A2", "%A3", "%A4", "%A5", "%A6", "%A7", "%A8", "%A9", "%AA", "%C0", "%C1", "%C2", "%C3", "%C4", "%C5", "%C6", "%C7", "%C8", "%C9", "%CA", "%CB", "%CC", "%CD", "%CE", "%CF", "%D0", "%D1", "%D2", "%D3", "%D4", "%D5", "%D6", "%D9", "%DA", "%DB", "%DC", "%DD", "%DE", "%DF", "<sprite=13>", "<sprite=12>", "%E2", "<sprite=14>", "%E4", "%E5", "%E6", "%E7", "%E8", "<sprite=15>", "%EA", "%EB", "%EC", "%ED", "%EE", "%EF", "%F0", "%F1", "%F2", "%F3", "%F4", "%F5", "%F6", "%F9", "%FA", "%FB", "%FC", "%FD", "%FE", "%FF" };

      for (var i = 0; i < a.Length; i++)
      {
        raw = raw.Replace(a[i], b[i]);
      }
    }
    return raw;
  }

  //check what is the description given for the user to complete the molecule
  private string CheckMoleculeDescription(string rawDisplay)
  {
    string result = "";

    if(rawDisplay.IndexOf("&") != -1)
    {
      string[] charc = rawDisplay.Split('&');
      result = "sabendo que é um " + charc[0] +"\n" +"e o seu nome é " + charc[1];
    }
    else if(rawDisplay.Contains("estrutura"))
    {
      result = "sabendo a sua estrutura: "+ CheckMoleculeRepresentation(rawDisplay);
    }
    else
    {
      result = "sabendo que é " + rawDisplay;
    }
    return result;
  }

  //if it is an structure , the function is going to read the file and get the string to display,
  //if not it just returns the raw display
  private string CheckMoleculeRepresentation(string rawDisplay)
  {
    string result = rawDisplay;
    int length = rawDisplay.Length;
    if (length > 9)
    {
      string sub = rawDisplay.Substring(rawDisplay.Length - 9, 9);
      if(sub == "estrutura")
      {
        result = HandleTextFile.ReadMoleculeStructure(rawDisplay);
        result = result.Replace("=-", " <sprite=1>");
        result = result.Replace("=", " <sprite=2>");
        result = result.Replace("|||", " <sprite=4>");
        result = result.Replace("||", " <sprite=3>");
        result = result.Replace("|", "<sprite=11>");
        result = result.Replace("///", " <sprite=5>");
        result = result.Replace("//", " <sprite=6>");
        result = result.Replace("/", " <sprite=7>");
        result = result.Replace("\\\\\\", " <sprite=8>");
        result = result.Replace("\\\\", "  <sprite=9>");
        result = result.Replace("\\", " <sprite=10>");
            
        string[] split = result.Split('\n');
        string tempResult = "\n";
        for (int i = 0; i < split.Length; i++)
        {
          string s = split[i];
          string s1 = split[i];
          //Debug.Log(s + " " + (s[0] == ' ') + " " + (s.IndexOf("C") == -1));
          if (s[0] == ' ' && s.IndexOf("H") == -1) //lines with | or \ or / need extra space
          {
            s1 = s.Insert(0, " ");
          }
          else if(s[0] == ' ' && s.IndexOf("H") != -1) //lines where they have H but not at the start, needs extra space
          {
            //s1 = s.Insert(0, " ");
            s1 = s1.Replace("H", " H");
          }
          else if((s[0]== 'H' || s[0] == ' ') && s.IndexOf("C") == -1)  //line with H and no C needs extra space at each H
          {
            s1 = s.Replace("H", " H ");
          }
          tempResult += s1 + "\n";
        }
        //the font doesnt have the | char, the 1 is the replacement
        result = tempResult;
      }
    }
    return result;
  }

  void NewDisplay()
  {
    check.SetActive(false);
    objectivesText.text = updatedDisplay;
  }

}
