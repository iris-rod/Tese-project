using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadButton1 : MonoBehaviour {

private Animator animator;
  private bool m_Started;
  private GameObject molecule;

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
        if (colliders[i].transform.name.Split(' ')[0] == "Contact" && molecule != null)
        {
          transform.parent.transform.parent.GetComponent<ShelfManager>().LoadMolecule(molecule);
          animator.SetBool("pushed",true);
          Invoke("Reset", .5f);
          break;
        }
      }
    }
  }

  void Reset()
  {
    animator.SetBool("pushed", false);
  }
  
  public void SetMolecule (GameObject mol)
  {
    molecule = mol;
  }
  
  public bool HasMolecule ()
  {
    return (molecule != null);
  }
}
