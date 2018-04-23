using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

  private Vector3 spawnPoint;
  private Collider[] hitColliders;
  private bool canSpawnAtom;
  private Vector3 hitScale;

	// Use this for initialization
	void Start () {
    hitScale = new Vector3(0.15f,.15f,.15f);
    canSpawnAtom = true;
    spawnPoint = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
    CanSpawnNewAtom();
	}

  public bool IsFree()
  {
    return canSpawnAtom;
  }

  void CanSpawnNewAtom()
  {
    Collider[] hitColliders = Physics.OverlapBox(spawnPoint, hitScale);
    for (int i = 0; i < hitColliders.Length; i++)
    {
      if (hitColliders[i].CompareTag("Interactable") || hitColliders[i].CompareTag("Bond"))
      {
        canSpawnAtom= false;
        return;
      }
    }
    canSpawnAtom =  true;
  }
}
