using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButton : MonoBehaviour {

  private Animator animator;
  private bool canPress, canUpdate;
  private MoleculeManager MM;
  private GameManager GM;

  public string obj;

	// Use this for initialization
	void Start () {
    canPress = true;
    canUpdate = true;
    animator = GetComponent<Animator>();
    MM = GameObject.Find("GameManager").GetComponent<MoleculeManager>();
    GM = GameObject.Find("GameManager").GetComponent<GameManager>();
  }
	
	// Update is called once per frame
	void Update () {
    CheckCollision();
    ResetTask();
	}

  private void CheckCollision ()
  {
    bool cleared = false;
    Collider[] colliders = Physics.OverlapBox (transform.position, transform.localScale / 5);
    if (colliders.Length > 1)
    {
      for (int i = 0; i < colliders.Length; i++)
      {
        if (colliders[i].transform.name.Split(' ')[0] == "Contact" && canPress)
        {

          if (obj.ToLower() == "molecule")
          {
            GameObject[] molecules = GameObject.FindGameObjectsWithTag("Molecule");
            for (int j = 0; j < molecules.Length; j++)
            {
              if (molecules[j].name.Split('_')[0] != "Mini")
                Destroy(molecules[j]);
            }
            cleared = true;
            MM.Clear();
            GameObject[] bonds = GameObject.FindGameObjectsWithTag("Bond");
            for (int k = 0; k < bonds.Length; k++)
            {
              if (bonds[k].transform.parent == null)
                Destroy(bonds[k]);
            }
          }
          else if (obj.ToLower() == "atom")
          {
            GameObject[] atoms = GameObject.FindGameObjectsWithTag("Interactable");
            for (int j = 0; j < atoms.Length; j++)
            {
              if (atoms[j].transform.parent == null)
                Destroy(atoms[j]);
            }
            cleared = true;
          }
          if (canPress)
          {
            SoundEffectsManager.PlaySound("button");
            canPress = false;
            if (cleared)
            {
              GM.RestorePartial();
              GM.UpdatePointSystem();
            }
          }
          animator.SetBool("pushed", true);
          Invoke("Reset", .5f);
          break;
        }
      }
    }
  }


  void ResetTask()
  {

  }

  void Reset()
  {
    animator.SetBool("pushed", false);
    canPress = true;
  }
}
