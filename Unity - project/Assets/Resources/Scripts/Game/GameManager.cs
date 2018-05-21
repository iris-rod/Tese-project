using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

  private int level;
  private bool levelComplete, newLevel;
  private Manager manager;

  // Use this for initialization
  void Start ()
  {
    manager = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Manager> ();
    levelComplete = false;
    newLevel = true;
    level = 0;
  }
	
  // Update is called once per frame
  void Update ()
  {
    if (newLevel)
      SetLevel ();
      
    if (levelComplete) {
      newLevel = true;
      level++;
    }
  }

  private void SetLevel ()
  {
    switch (level) {
    case 1:
      manager.LoadMolecule ("level_1", false);
      break;
    case 2:
      manager.LoadMolecule ("level_2", false);
      break;
    case 3:
      manager.LoadMolecule ("level_3", false);
      break;
    case 4:
      manager.LoadMolecule ("level_4", false);
      break;
    case 5:
      manager.LoadMolecule ("level_5", false);
      break;
    
    }
    
    newLevel = false;
  }
  
  public void LevelCompleted ()
  {
    levelComplete = true;
  }
  
}
