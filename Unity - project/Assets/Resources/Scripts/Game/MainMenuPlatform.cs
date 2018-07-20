using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlatform : MonoBehaviour {

  private Vector3 spawnPoint;
  private MainMenuManager MMM;

	// Use this for initialization
	void Start () {
    spawnPoint = new Vector3(transform.position.x, transform.position.y + .1f, transform.position.z);//new Vector3(platformPosition.x, platformPosition.y + 0.1f, platformPosition.z);
    MMM = GameObject.Find("MainMenuManager").GetComponent<MainMenuManager>();
  }

  // Update is called once per frame
  void Update () {
    Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 0.01f);
    if (hitColliders.Length > 1)
    {
      for (int i = 0; i < hitColliders.Length; i++)
      {
        if (hitColliders[i].CompareTag("Interactable"))
        {
          string nextScene = hitColliders[i].GetComponent<MainMenuBall>().GetNextScene();
          string levels = hitColliders[i].GetComponent<MainMenuBall>().GetLevels();
          MMM.ChangeScene(nextScene, levels);
          
        }          
      }
    }
  }
}
