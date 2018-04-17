using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour {

  private int moleculeID;
  private bool canSave;
 
  // Use this for initialization
  void Start () {
    moleculeID = 1;
    canSave = true;
	}
	
	// Update is called once per frame
	void Update ()
  { 


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
    GetComponent<InterfaceManager>().Load(null,"saved_" + moleculeID);
    GameObject mini = GameObject.Find("Mini_saved_"+(moleculeID).ToString());
    //mini.transform.localScale -= new Vector3(4f, 4f, 4f);
    //mini.transform.parent = transform;
    mini.transform.position = new Vector3(0,1.2f,.1f);
    mini.transform.localScale -= new Vector3(0.6f, 0.6f, 0.6f);
    mini.gameObject.AddComponent(typeof(BoxCollider));

  }
}
