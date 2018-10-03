using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundEffectsManager
{

  private static AudioSource sourceAtoms;
  private static AudioSource sourceHands;

  private static AudioClip AtomsTouch;
  private static AudioClip MoleculeRotation;
  private static AudioClip AtomsBonded;
  private static AudioClip BondBreak;
  private static AudioClip DefaultButton;
  private static AudioClip AnswerButton;
  private static AudioClip AtomGrabbed;
  private static AudioClip ShelfSlidderOpen;
  private static AudioClip ShelfSlidderClose;
  private static AudioClip Trash;


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
        sourceAtoms.time = 1f;
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

  public static void SetLoop(bool value)
  {
    sourceAtoms.loop = value;
  }
}
