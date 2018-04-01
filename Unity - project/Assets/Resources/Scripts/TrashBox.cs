using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  void OnTriggerEnter(Collider col)
  {
    if (col.CompareTag("Interactable"))
    {
      //
      if (col.transform.parent == null)
        Destroy(col.transform.gameObject);
      else
        Destroy(col.transform.parent.gameObject);
    }
  }
}
