using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButton : MonoBehaviour {

  private Animator animator;

	// Use this for initialization
	void Start () {
    animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	}

  void OnTriggerEnter(Collider col)
  {
    string name = col.name.Split(' ')[0];
    if(name == "Contact")
    {
     /* GameObject[] molecules = GameObject.FindGameObjectsWithTag("Molecule");
      for(int i = 0; i < molecules.Length; i++)
      {
        if(molecules[i].name.Split('_')[0] != "Mini")
          Destroy(molecules[i]);
      }*/
    }
    Debug.Log("here");
    animator.SetBool("pushed",true);
    Invoke("Reset", .5f);
  }

  void Reset()
  {
    animator.SetBool("pushed", false);
  }
}
