using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleMoleculeBehaviour : MonoBehaviour {

  private bool hasOverlap;
  private GameObject overlapGO;
  private Vector3 pos,sca;
  private string task;
  private MoleculeManager MM;
  private Manager M;
  private int number,test;
	// Use this for initialization
	void Start () {
    test = 5;
		hasOverlap = false;
    MM = GameObject.Find("GameManager").GetComponent<MoleculeManager>();
    M = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Manager>();
	}
	
  void Update () {
    CheckCollision();
  }
  public void SetNumber(int n)
  {
    number = n;

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
    switch (task)
    {
      case "move_etanol":
        pos = new Vector3(transform.position.x+.1f, transform.position.y + 2.1f, transform.position.z + .36f);
        sca = new Vector3(.5f, .5f, .19f);
        break;
      case "move_co2":
        pos = new Vector3(transform.position.x, transform.position.y + 2.1f, transform.position.z + .33f);
        sca = new Vector3(.3f, .3f, .19f);
        break;
      case "move_h2o":
        pos = new Vector3(transform.position.x+.01f, transform.position.y + 2.2f, transform.position.z + .37f);
        sca = new Vector3(.3f, .3f, .19f);
        break;
    }

  }

  private void CheckCollision ()
  {
    bool overlap = false;
    Collider[] colliders = Physics.OverlapBox (pos, transform.localScale / 10);
    if (colliders.Length > 1) {
      for (int i = 0; i < colliders.Length; i++) {
        if (colliders [i].transform.CompareTag("Interactable") && colliders[i].transform.parent != null ){//&& CheckMoleculesMatch(colliders[i].transform.parent)) {

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

 /* private void OnDrawGizmos()
  {
    Vector3 scal = new Vector3(.3f,.3f,.19f);
    Gizmos.DrawWireCube(pos, scal);
  }
  */
  public bool HasOverlap ()
  {
    return hasOverlap;
  }
  
  public void DestroyOverlap ()
  {
    M.SaveMolecule(overlapGO,"test_" + test + "mol_" + number );
    Destroy(overlapGO);
  }
}
