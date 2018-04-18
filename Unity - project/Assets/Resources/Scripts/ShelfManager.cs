using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour {

  private int moleculeID;
  private bool canSave;
  private GameObject platform;
 
  private GameObject mini;
  private bool newMini;
  private float zOffset, yOffset;

  // Use this for initialization
  void Start () {
    platform = GameObject.Find("InviPlatform");
    moleculeID = 1;
    newMini = true;
    canSave = true;
	}
	
	// Update is called once per frame
	void Update ()
  {
    if (mini != null && newMini)
    {
      mini.transform.position = new Vector3(-0.5f,1.6f,-.25f +((moleculeID-1) * .08f));//0,1.2,.1
      BoxCollider col = mini.GetComponent<BoxCollider>();
      col.size = new Vector3(.5f,.5f,.5f);
      col.center = new Vector3(0,2,.3f);
      col.isTrigger = true;
      mini.transform.localScale -= new Vector3(0.8f, 0.8f, 0.8f);
      mini.transform.parent = transform;
      newMini = false;
    }
  }


  void OnTriggerEnter(Collider col)
  {
    if (col.CompareTag("Interactable") && canSave)
    {
      GetComponent<InterfaceManager>().Save(col.transform.parent.gameObject, "saved_" + moleculeID.ToString());
      CreateMiniMolecule(col.transform.parent);
      canSave = false;
      Invoke("ResetSave",1f);
      moleculeID++;
    }
  }

  void ResetSave()
  {
    canSave = true;
  }

  private void CreateMiniMolecule(Transform molecule)
  {
    GetComponent<InterfaceManager>().Load(true,"saved_" + moleculeID);
    mini = GameObject.Find("Mini_saved_"+(moleculeID).ToString());
    mini.gameObject.AddComponent(typeof(BoxCollider));
    newMini = true;
  }

  public void LoadMolecule(GameObject molecule)
  {
    if (platform.GetComponent<Platform>().IsFree())
    {
      string name = molecule.name.Split('_')[1] + "_" + molecule.name.Split('_')[2];
      GetComponent<InterfaceManager>().Load(false, name);
    }
  }
}
