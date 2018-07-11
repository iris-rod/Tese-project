using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class TrashBox : MonoBehaviour {

  private MoleculeManager MM;

	// Use this for initialization
	void Start () {
    MM = GameObject.Find("GameManager").GetComponent<MoleculeManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  void OnTriggerEnter(Collider col)
  {
    if ((col.CompareTag("Interactable") || col.CompareTag("Pivot")) && col.transform.GetComponent<InteractionBehaviour>().isGrasped)
    {
      if (col.transform.parent == null)
        Destroy(col.transform.gameObject);
      else
      {
        MM.RemoveMolecule(col.transform.parent.gameObject);
        Destroy(col.transform.parent.gameObject);
      }
      SoundEffectsManager.PlaySound("throwTrash");
    }
  }
}
