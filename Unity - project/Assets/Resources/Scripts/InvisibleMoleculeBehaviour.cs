using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleMoleculeBehaviour : MonoBehaviour {

  private bool hasOverlap;
  private GameObject overlapGO;
  private Vector3 pos;
  private string task;
  private MoleculeManager MM;

	// Use this for initialization
	void Start () {
		hasOverlap = false;
    MM = GameObject.Find("GameManager").GetComponent<MoleculeManager>();
	}
	
  void Update () {
    CheckCollision();
  }
  
  public void SetTask (string t)
  {
    task = t;
    if (task == "move_2")
      pos = new Vector3 (transform.position.x + .4f, transform.position.y + 2.2f, transform.position.z + .3f);
    else if (task == "move_1")
      pos = new Vector3 (transform.position.x, transform.position.y + 2.4f, transform.position.z + .4f);
    else {
      pos = new Vector3 (transform.position.x, transform.position.y + 2f, transform.position.z + .4f);
    }
  }

  private void CheckCollision ()
  {
    bool overlap = false;
    Collider[] colliders = Physics.OverlapBox (pos, transform.localScale / 10);
    if (colliders.Length > 1) {
      for (int i = 0; i < colliders.Length; i++) {
        if (colliders [i].transform.CompareTag("Interactable") && colliders[i].transform.parent != null ) {//&& CheckMoleculesMatch(colliders[i].transform.parent)) {

          overlap = true;
          overlapGO = colliders[i].transform.parent.gameObject;
        }
      }
    }
    hasOverlap = overlap;
  }

  private bool CheckMoleculesMatch(Transform molecule)
  {
    if (molecule.childCount != transform.childCount)
      return false;

    return MM.CompareMolecules(molecule.gameObject, transform.gameObject);

  }
  
  public bool HasOverlap ()
  {
    return hasOverlap;
  }
  
  public void DestroyOverlap ()
  {
    Destroy(overlapGO);
  }
}
