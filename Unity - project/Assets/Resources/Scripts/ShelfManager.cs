using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour {

  private bool VR;
  private bool Buttons;
  private int moleculeID;
  private bool canSave;
  private GameObject platform;
 
  private GameObject mini;
  private bool newMini;
  private string lastPointedMini;
  private bool waiting;

  private float yShelfPosition;
  private int zOffset;
  
  private GameObject[] spots;

  // Use this for initialization
  void Start () {
    VR = true;
    Buttons = true;
    platform = GameObject.Find("InviPlatform");
    moleculeID = 1;
    zOffset = moleculeID;
    newMini = true;
    canSave = true;
    waiting = false;
    lastPointedMini = "";
    if(!VR)
      yShelfPosition = 1.6f-.04f;
    else yShelfPosition = .85f;
    spots = GameObject.FindGameObjectsWithTag("Interface");
	}
	
	// Update is called once per frame
	void Update ()
  {
    if (mini != null && newMini) {
      Vector3 pos = GetMiniPosition ();
      mini.transform.position = pos;
      //desktop version values for the mini mols on the shelves
      if (!VR) {
        mini.transform.localScale -= new Vector3 (0.8f, 0.8f, 0.8f);
      }
      //VR version values for the mini mols on the shelves
      else {
        mini.transform.localScale -= new Vector3 (0.2f, 0.2f, 0.2f);
      }
      mini.transform.parent = transform;
      newMini = false;
      zOffset++;
    }
  }

  private Vector3 GetMiniPosition ()
  {
    SetMoleculeOnSpot();
    if ((moleculeID - 1) % 4 == 0) {
      yShelfPosition -= .3f;//desktop->0.1f;
      zOffset = 1;
    }
    if (!VR)
      return new Vector3 (-0.45f, yShelfPosition, -.2f + ((zOffset - 1) * .1f)); //z -> -0.25
    else
      return new Vector3 (-0.23f, yShelfPosition, -.2f + ((zOffset - 1) * .25f)); 
    
  }
  
  private void SetMoleculeOnSpot ()
  {
    for (int i = 0; i < spots.Length; i++) {
      if (!spots [i].GetComponent<LoadButton1> ().HasMolecule()) {
        spots [i].GetComponent<LoadButton1> ().SetMolecule (mini);
        return;
      }
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
    if (!waiting && platform.GetComponent<Platform>().IsFree())
    {
      string name = molecule.name.Split('_')[1] + "_" + molecule.name.Split('_')[2];
      GetComponent<InterfaceManager>().Load(false, name);
      waiting = true;
      Invoke("Wait",1);
    }
  }

  void Wait()
  {
    waiting = false;
  }

  public void LoadShelf (Vector3 fingerPos, Vector3 fingerDir)
  {
    RaycastHit hit;
    if (Physics.Raycast (fingerPos, fingerDir, out hit, Mathf.Infinity)) {
      Debug.DrawRay (fingerPos, fingerDir * hit.distance, Color.yellow);
      
      if (hit.transform.parent != null) {      
        string first = hit.transform.parent.name.Split ('_') [0];
      
        //if detect the mini molecule (the parent is the shelf)
        if (first == "shelves" && hit.transform.name != lastPointedMini) {
          if (lastPointedMini != "")
            GameObject.Find (lastPointedMini).GetComponent<Molecule> ().HighlightMini (false);  
          hit.transform.GetComponent<Molecule> ().HighlightMini (true);
          lastPointedMini = hit.transform.name;
        }
      
        //if detect a atom or other component from the mini molecule      
        if (first == "Mini" && hit.transform.parent.name != lastPointedMini) { //lastPointedMini != "" &&
          if (lastPointedMini != "")
            GameObject.Find (lastPointedMini).GetComponent<Molecule> ().HighlightMini (false);  
          hit.transform.parent.GetComponent<Molecule> ().HighlightMini (true);//LoadMolecule(hit.transform.gameObject);
          lastPointedMini = hit.transform.parent.name;   
        }
      }
    }
  }
  
  public void CancelLoad ()
  {
    if(lastPointedMini != "")
      GameObject.Find (lastPointedMini).GetComponent<Molecule> ().HighlightMini (false);      
  }
}
