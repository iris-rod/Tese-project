using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour {

  private int moves;
  private bool countingMoves;

  //variables to count time
  private float time;
  private float initTime;
  private bool countingTimer;

	// Use this for initialization
	void Start () {
    countingTimer = false;
    countingMoves = false;
	}
	
	// Update is called once per frame
	void Update () {
    if (countingTimer)
    {
      float currentTime = Time.realtimeSinceStartup;
      float timePassed = currentTime - initTime;
      int min = Mathf.FloorToInt(timePassed / 60);
      int sec = Mathf.FloorToInt(timePassed % 60);
    }
	}

  public void StartTimer()
  {
    initTime = Time.realtimeSinceStartup;
    countingTimer = true;
  }

  public void StartMovesCounter()
  {
    countingMoves = true;
  }

  public void StopTimer()
  {
    countingTimer = false;
  }

  public void StopMovesCounter()
  {
    moves = 0;
    countingMoves = false;
  }

  public void UpdateMoves()
  {
    if (countingMoves)
      moves++;
  }

}
