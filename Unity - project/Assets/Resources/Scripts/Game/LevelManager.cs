using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

  private int id;
  private int sublevel;
  private int maxSubLevels;
	private bool completed, onGoing;
  private string objective;
  private Dictionary<string, bool> objectives;
  private GameManager GM;
      
  // Use this for initialization
	void Start () {
    GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    objectives = new Dictionary<string, bool>();
		completed = false;
    onGoing = true;
    sublevel = 1;
	}
  
  
  ////PUBLIC FUNCTIONS
  public void SetLevelId (int value)
  {
    id = value;
  }
  
  public void SetObjective (string obj )
  {
    objectives = new Dictionary<string, bool>();
    string[] objs = obj.Split('_');
    for(int i = 0; i < objs.Length; i++)
    {
      objectives.Add(objs[i],false);
    }
    maxSubLevels = objectives.Count;
    sublevel = 1;
  }
  
  public void UpdateObjective(string obj)
  {
    foreach(var par in objectives)
    {
      if (par.Key == obj)
      {
        objectives[par.Key] = true;
        sublevel++;
        break;
      }
      else if (!par.Value)
        break;
    }
  }

  public Dictionary<string, bool> GetObjectives()
  {
    return objectives;
  }

  public string GetNextObjective()
  {
    foreach (var par in objectives)
    {
      if (!par.Value)
      {
        return par.Key;
      }
    }
    return "";
  }

  public bool LevelCompleted()
  {
    CheckCompletion();
    return completed;
  }

  private void CheckCompletion()
  {
    completed = (maxSubLevels < sublevel);
    /*bool allDone = true;
    foreach (var par in objectives)
    {
      if (!par.Value)
        allDone = false;
    }
    completed = allDone;*/
  }

  public int GetSublevel()
  {
    return sublevel;
  }

  public int GetLevel()
  {
    return id;
  }


  
  
  
}
