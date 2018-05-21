using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

  private int id;
	private bool completed, onGoing;
  private string objective;
      
  // Use this for initialization
	void Start () {
		completed = false;
    onGoing = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
  
  
  ////PUBLIC FUNCTIONS
  public void SetLevelId (int value)
  {
    id = value;
  }
  
  public void SetObjective (string obj )
  {
    
  }
  
  public void LevelCompleted ()
  {
    //CheckCompletion();
  }
  
  
  
}
