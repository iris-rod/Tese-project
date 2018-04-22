using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour {

  private int moleculeID;
  private bool canSave;
  private GameObject platform;
 
  private GameObject mini;
  private bool newMini;

  private float yShelfPosition;
  private int zOffset;

  // Use this for initialization
  void Start () {
    platform = GameObject.Find("InviPlatform");
    moleculeID = 1;
    zOffset = moleculeID;
    newMini = true;
    canSave = true;
    yShelfPosition = 1.6f-.04f;
	}
	
	// Update is called once per frame
	void Update ()
  {
    if (mini != null && newMini)
    {
      Vector3 pos = GetMiniPosition();
      mini.transform.position = pos;
      BoxCollider col = mini.GetComponent<BoxCollider>();
      col.size = new Vector3(.5f,.5f,.5f);
      col.center = new Vector3(0,2,.1f);
      col.isTrigger = true;
      mini.transform.localScale -= new Vector3(0.8f, 0.8f, 0.8f);
      mini.transform.parent = transform;
      newMini = false;
      zOffset++;
    }
  }

  private Vector3 GetMiniPosition()
  {
    if ((moleculeID-1) % 5 == 0)
    {
      yShelfPosition -= 0.1f;
      zOffset = 1;
    }
    return new Vector3(-0.45f, yShelfPosition, -.2f + ((zOffset - 1) * .1f)); //z -> -0.25
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
    for(int i = 0; i < transform.childCount; i++)
    {
      Transform child = transform.GetChild(i);
      if (child.CompareTag("Interactable"))
        child.GetComponent<InteractionBehaviour>().ignoreGrasping = false;
    }
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

  public void LoadShelf(Vector3 fingerPos, Vector3 fingerDir)
  {
    RaycastHit hit;

    if(Physics.Raycast(fingerPos,fingerDir, out hit, Mathf.Infinity))
    {
      Debug.DrawRay(fingerPos, fingerDir * hit.distance, Color.yellow);
      Debug.Log(hit.transform.name);
      if(hit.transform.name.Split('_')[0] == "Mini")
      {
        LoadMolecule(hit.transform.gameObject);
      }
    }

  }
}
