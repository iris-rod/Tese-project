using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour {

  private int points;
  private int movesStep1;
  private int movesStep2;
  private int movesStep3;
  private int timeStep1;
  private int timeStep2;
  private int timeStep3;

  //variables to count moves
  private int moves;
  private bool countingMoves;
  private int minimumMoves;

  //variables to count time
  private float time;
  private float initTime;
  private bool countingTimer;
  private int min, sec;

  private InformationManager IM;

	// Use this for initialization
	void Start () {
    countingTimer = false;
    countingMoves = false;
    IM = GameObject.FindGameObjectWithTag("Board").transform.GetChild(0).GetComponent<InformationManager>();
    timeStep1 = 5;
    timeStep2 = 15;
    timeStep3 = 30;

  }
	
	// Update is called once per frame
	void Update () {
    if (countingTimer)
    {
      float currentTime = Time.realtimeSinceStartup;
      float timePassed = currentTime - initTime;
      min = Mathf.FloorToInt(timePassed / 60);
      sec = Mathf.FloorToInt(timePassed % 60);
      IM.UpdateTimer(min,sec);
    }
	}

  private void SetMinimumMoves(int min)
  {
    minimumMoves = min;
    movesStep1 = minimumMoves;
    movesStep2 = minimumMoves + 3;
    movesStep3 = minimumMoves + 6;
  }

  public void StartTimer()
  {
    initTime = Time.realtimeSinceStartup;
    countingTimer = true;
    IM.UpdateTimer(0, 0);
  }

  public void StartMovesCounter()
  {
    moves = 0;
    countingMoves = true;
    IM.UpdateMoves(moves);
  }

  public void StopTimer()
  {
    SetPoints();
    countingTimer = false;
  }

  public void StopMovesCounter()
  {
    SetPoints();
    countingMoves = false;
  }

  public void UpdateMoves()
  {
    if (countingMoves)
    {
      moves++;
      IM.UpdateMoves(moves);
    }
  }

  public void StopAll()
  {
    StopMovesCounter();
    StopTimer();
  }

  public int GetMoves()
  {
    return moves;
  }

  public string GetTime()
  {
    return min + ":" + sec;
  }

  public int GetPoints()
  {
    return points;
  }

  private void SetPoints()
  {
    if (countingMoves)
    {
      if (moves == movesStep1)
        points += 4;
      else if (moves > movesStep1 && moves <= movesStep2)
        points += 3;
      else if (moves > movesStep2 && moves <= movesStep3)
        points += 2;
      else if (moves > movesStep3)
        points += 1;
    }
    else if (countingTimer)
    {
      if (sec <= timeStep1)
        points += 4;
      else if (sec > timeStep1 && sec <= timeStep2)
        points += 3;
      else if (sec > timeStep2 && sec <= timeStep3)
        points += 2;
      else if (sec > timeStep3)
        points += 1;
    }
   }
  }


