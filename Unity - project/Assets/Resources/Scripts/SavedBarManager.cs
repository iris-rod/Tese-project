//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SavedBarManager : MonoBehaviour {

//  private GameObject camera;
//  private int maxNumberChild = 5;

//  public GameObject savedMoleculePrefab;

//  // Use this for initialization
//  void Start () {
//    camera = GameObject.FindGameObjectWithTag("MainCamera");
//	}

//  void CheckLoadDoubleTouch()
//  {
//    bool check = true;
//    int selectedID = -1;
//    for (int i = 0; i < transform.childCount; i++)
//    {
//      Transform child = transform.GetChild(i);
//      if (!check)
//        child.GetComponent<SavedMolecule>().SetOneSelected(true);
//      if (child.GetComponent<SavedMolecule>().IsSelected())
//      {
//        check = false;
//        selectedID = i;
//      }
//    }
//    camera.GetComponent<Manager>().SetCanLoad(check);
//    if (check)
//    {
//      for (int i = 0; i < transform.childCount; i++)
//      {
//        Transform child = transform.GetChild(i);
//        child.GetComponent<SavedMolecule>().SetOneSelected(false);
//      }
//    }
//    else
//    {
//      for (int i = 0; i < transform.childCount; i++)
//      {
//        if (i != selectedID)
//        {
//          Transform child = transform.GetChild(i);
//          child.GetComponent<SavedMolecule>().SetOneSelected(true);
//        }
//      }
//    }
//  }

//  void CheckLoadOneTouch()
//  {
//    for (int i = 0; i < transform.childCount; i++)
//    {
//      Transform child = transform.GetChild(i);

//    }
//  }

//  public void CheckSelectedItems(GameObject newSelected)
//  {
//    for (int i = 0; i < transform.childCount; i++)
//    {
//      Transform child = transform.GetChild(i);
//      if (!GameObject.ReferenceEquals(child, newSelected) && child.GetComponent<SavedMolecule>().IsSelected())
//      {
//        child.GetComponent<SavedMolecule>().SetSelected(false);
//      }
//    }
//  }

//  public void SetMoleculeOnChild(string fileName)
//  {
//    if (transform.childCount < maxNumberChild)
//    {
//      Vector3 pos = new Vector3(transform.position.x + (transform.childCount * 0.2f + 0.1f), transform.position.y, transform.position.z);
//      GameObject saved = Instantiate(savedMoleculePrefab, pos, transform.rotation);
//      saved.transform.parent = transform;
//      saved.GetComponent<SavedMolecule>().SetFileName(fileName);
//      /*if(transform.parent.transform.parent != null) //if it is the notebook
//      {
//        saved.transform.localScale = new Vector3(.5f, .2f, .2f);
//        saved.transform.position = new Vector3(-0.53f,-0.03f -(transform.childCount*.2f + .1f) ,-.8f);
//      }*/
//    }
//  }
//}
