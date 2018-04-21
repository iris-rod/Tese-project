using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButton : MonoBehaviour {

  private Animator animator;
  private bool m_Started;

	// Use this for initialization
	void Start () {
    m_Started = true;
    animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
    CheckCollision();
	}

  private void CheckCollision()
  {
    Collider[] colliders = Physics.OverlapBox(transform.position,transform.localScale/2);
    if(colliders.Length > 1)
    {
      for(int i = 0; i < colliders.Length; i++)
      {
        if (colliders[i].transform.name.Split(' ')[0] == "Contact")
        {
           GameObject[] molecules = GameObject.FindGameObjectsWithTag("Molecule");
           for(int j = 0; j < molecules.Length; j++)
           {
             if(molecules[j].name.Split('_')[0] != "Mini")
               Destroy(molecules[j]);
           }
          animator.SetBool("pushed",true);
          Invoke("Reset", .5f);
          break;
        }
      }
    }
  }

  void OnCollisionEnter(Collision col)
  {
    string name = col.transform.name.Split(' ')[0];
    if(name == "Contact")
    {
     /* GameObject[] molecules = GameObject.FindGameObjectsWithTag("Molecule");
      for(int i = 0; i < molecules.Length; i++)
      {
        if(molecules[i].name.Split('_')[0] != "Mini")
          Destroy(molecules[i]);
      }*/
    }
    //Debug.Log("here");
    //animator.SetBool("pushed",true);
    //Invoke("Reset", .5f);
  }

  void Reset()
  {
    animator.SetBool("pushed", false);
  }
}
