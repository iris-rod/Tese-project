using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAtomsManager : MonoBehaviour
{

  public GameObject[] FirstRowRef;
  public GameObject[] SecondRowRef;
  public GameObject[] ThirdRowRef;

  private int rowDisplay;
  private bool start = true;

  // Use this for initialization
  void Start()
  {
    rowDisplay = 1;

  }

  public void SetUp()
  {
    for (int i = 0; i < SecondRowRef.Length; i++)
    {
      SecondRowRef[i].transform.GetChild(1).GetComponent<BoxAtoms>().Move(true);
    }
    for (int i = 0; i < ThirdRowRef.Length; i++)
    {
      ThirdRowRef[i].transform.GetChild(1).GetComponent<BoxAtoms>().Move(true);
    }
  }

  public void ChangeRow()
  {
    rowDisplay++;
    if (rowDisplay > 3)
      rowDisplay = 1;
    Debug.Log(rowDisplay);
    ChangeBoxRow();
  }

  private void ChangeBoxRow()
  {
    switch (rowDisplay)
    {
      case 1:
        for (int i = 0; i < ThirdRowRef.Length; i++)
        {
          ThirdRowRef[i].transform.GetChild(1).GetComponent<BoxAtoms>().Move(true);
        }
        for (int i = 0; i < FirstRowRef.Length; i++)
        {
          FirstRowRef[i].transform.GetChild(1).GetComponent<BoxAtoms>().Move(true);
        }
        break;
      case 2:
        for (int i = 0; i < FirstRowRef.Length; i++)
        {
          FirstRowRef[i].transform.GetChild(1).GetComponent<BoxAtoms>().Move(true);
        }
        for (int i = 0; i < SecondRowRef.Length; i++)
        {
          SecondRowRef[i].transform.GetChild(1).GetComponent<BoxAtoms>().Move(true);
        }
        break;
      case 3:
        for (int i = 0; i < SecondRowRef.Length; i++)
        {
          SecondRowRef[i].transform.GetChild(1).GetComponent<BoxAtoms>().Move(true);
        }
        for (int i = 0; i < ThirdRowRef.Length; i++)
        {
          ThirdRowRef[i].transform.GetChild(1).GetComponent<BoxAtoms>().Move(true);
        }
        break;
    }
    
  }

  void Update()
  {
    if (start) SetUp();
    start = false;
    if (Input.GetKeyDown(KeyCode.Space))
    {
      ChangeRow();
    }
  }

}
