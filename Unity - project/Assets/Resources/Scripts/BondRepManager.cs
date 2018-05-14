using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondRepManager : MonoBehaviour {

  private int MaxNumberOfBonds;
  private int numberOfBondsAvailable;
  
  private int lastNumberAvailable;
  
	// Use this for initialization
	void Start () {
    MaxNumberOfBonds = GetComponent<Atom>().GetMaxNumberOfBonds();
    numberOfBondsAvailable = MaxNumberOfBonds;
    lastNumberAvailable = -1;
    //lastNumberAvailable = numberOfBondsAvailable;
    //Debug.Log(GetComponent<Atom>().GetAtomType() + " " + GetComponent<Atom> ().GetNumberBondsMade ());
	}
	
	// Update is called once per frame
	void Update ()
  {
    numberOfBondsAvailable = MaxNumberOfBonds - GetComponent<Atom> ().GetNumberBondsMade ();
    if (lastNumberAvailable != numberOfBondsAvailable) {
      UpdateRepresentation ();
      lastNumberAvailable = numberOfBondsAvailable;
    }
	}
  
  private void UpdateRepresentation ()
  {
    int num = 0;
    for (int i = 0; i < transform.childCount; i++) {
      Transform child = transform.GetChild (i);
      if (num < numberOfBondsAvailable) {
        child.gameObject.SetActive (true);
        num++;
      }      
      else
        child.gameObject.SetActive(false);
    }
  }
  
}
