using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestsManager : MonoBehaviour {

  private bool newTask;
  private bool start;
  private int lastKey;
  private string lastMol;

	// Use this for initialization
	void Start () {
    start = true;
    newTask = true;
    lastKey = -1;
	}
	
	// Update is called once per frame
	void Update () {
    if (start)
    {
      LogsC.Instance.startSession("default");
      start = false;
    }

    if (Input.GetKeyDown("0"))
    {
      CheckLastKey(1);
      if (newTask)
        LogsC.Instance.sessionNewTask("rotate UPDOWN auto");
      //else
      //  LogsC.Instance.sessionSubTaskReload();
    }

    if (Input.GetKeyDown("1"))
    {
      CheckLastKey(1);
      if(newTask)
        LogsC.Instance.sessionNewTask("rotate !UPDOWN auto");
      //else
      //  LogsC.Instance.sessionSubTaskReload();
    }

    if (Input.GetKeyDown("2"))
    {
      CheckLastKey(2);
      if (newTask)
        LogsC.Instance.sessionNewTask("rotate UPDOWN free");
      //else
      //  LogsC.Instance.sessionSubTaskReload();
    }

    if (Input.GetKeyDown("3"))
    {
      CheckLastKey(3);
      if (newTask)
        LogsC.Instance.sessionNewTask("rotate !UPDOWN free");
      //else
      //  LogsC.Instance.sessionSubTaskReload();
    }

    if (Input.GetKeyDown("4"))
    {
      CheckLastKey(4);
      if (newTask)
        LogsC.Instance.sessionNewTask("rotate pivot");
      //else
      //  LogsC.Instance.sessionSubTaskReload();
    }

    if (Input.GetKeyDown("5"))
    {
      CheckLastKey(5);
      if (newTask)
        LogsC.Instance.sessionNewTask("move pivot");
      //else
      //  LogsC.Instance.sessionSubTaskReload();
    }

    if (Input.GetKeyDown("6"))
    {
      CheckLastKey(6);
      if (newTask)
        LogsC.Instance.sessionNewTask("move atom");
      //else
      //  LogsC.Instance.sessionSubTaskReload();
    }

    if (Input.GetKeyDown("7"))
    {
      CheckLastKey(7);
      if (newTask)
        LogsC.Instance.sessionNewTask("connect dist");
      //else
      //  LogsC.Instance.sessionSubTaskReload();
    }

    if (Input.GetKeyDown("8"))
    {
      CheckLastKey(8);
      if (newTask)
        LogsC.Instance.sessionNewTask("connect taps");
      //else
      //  LogsC.Instance.sessionSubTaskReload();
    }

    if (Input.GetKeyDown(KeyCode.KeypadEnter))
    {
      LogsC.Instance.sessionNewSubTask();
    }

  }

  private void CheckLastKey(int key)
  {
    if (key != lastKey)
      newTask = true;
    else
      newTask = false;
    lastKey = key;
  }

  public void CheckReloadTask(string mol)
  {
    if(lastMol == mol)
    {
      LogsC.Instance.sessionSubTaskReload();
    }
    lastMol = mol;
  }
}
