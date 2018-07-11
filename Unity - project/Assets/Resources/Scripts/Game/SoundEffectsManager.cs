using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundEffectsManager
{

  private static AudioSource sourceAtoms;
  private static AudioSource sourceHands;

  public static AudioClip AtomsTouch;
  public static AudioClip MoleculeRotation;
  public static AudioClip AtomsBonded;
  public static AudioClip BondBreak;
  public static AudioClip DefaultButton;
  public static AudioClip AnswerButton;
  public static AudioClip AtomGrabbed;
  public static AudioClip ShelfSlidderOpen;
  public static AudioClip ShelfSlidderClose;
  public static AudioClip Trash;


  // Use this for initialization
  public static void SetUp()
  {
    AudioSource[] sources = GameObject.Find("GameManager").GetComponents<AudioSource>();
    sourceAtoms = sources[0];
    AtomsTouch = Resources.Load("Sound Effects/Atoms Touch", typeof(AudioClip)) as AudioClip;
    AtomsBonded = Resources.Load("Sound Effects/Atoms Bonded", typeof(AudioClip)) as AudioClip;
    AtomGrabbed = Resources.Load("Sound Effects/Grab Atom", typeof(AudioClip)) as AudioClip;
    BondBreak = Resources.Load("Sound Effects/Bond break", typeof(AudioClip)) as AudioClip;
    MoleculeRotation = Resources.Load("Sound Effects/Atom rotation", typeof(AudioClip)) as AudioClip;
    DefaultButton = Resources.Load("Sound Effects/Button", typeof(AudioClip)) as AudioClip;
    AnswerButton = Resources.Load("Sound Effects/Button Answer", typeof(AudioClip)) as AudioClip;
    ShelfSlidderOpen = Resources.Load("Sound Effects/Slide", typeof(AudioClip)) as AudioClip;
    ShelfSlidderClose = Resources.Load("Sound Effects/Slide 2", typeof(AudioClip)) as AudioClip;
    Trash = Resources.Load("Sound Effects/ThrowTrash", typeof(AudioClip)) as AudioClip;
  }

  public static void PlaySound(string action)
  {
    AudioClip sound = null;
    switch (action)
    {
      case "bondBreak":
        sound = BondBreak;
        
        break;
      case "atomsTouch":
        sound = AtomsTouch;
        break;
      case "rotation":
        sound = MoleculeRotation;
        break;
      case "atomsBonded":
        sound = AtomsBonded;
        break;
      case "button":
        sound = DefaultButton;
        break;
      case "buttonAnswer":
        sound = AnswerButton;
        break;
      case "atomGrabbed":
        sound = AtomGrabbed;
        break;
      case "shelfSlidderOpen":
        sound = ShelfSlidderOpen;
        break;
      case "shelfSlidderClose":
        sound = ShelfSlidderClose;
        break;
      case "throwTrash":
        sound = Trash;
        break;
    }
    if (sound != null)
      sourceAtoms.PlayOneShot(sound, .5f);
  }

  public static void StopAudio()
  {
    sourceAtoms.Stop();
  }
}
