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
  private int min, sec;

  private InformationManager IM;

	// Use this for initialization
	void Start () {
    countingTimer = false;
    countingMoves = false;
    IM = GameObject.FindGameObjectWithTag("Board").transform.GetChild(0).GetComponent<InformationManager>();
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
    countingTimer = false;
  }

  public void StopMovesCounter()
  {
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

}
