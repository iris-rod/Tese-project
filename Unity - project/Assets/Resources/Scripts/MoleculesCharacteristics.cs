using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoleculesCharacteristics {

  public static bool CheckTheClass(string classe, GameObject mol)
  {
    switch (classe)
    {
      case "hidrocarbonato":
        if (CheckOtherClasses("hidrocarbonato", mol)) return false;
        return CheckForHidrocarbonates(mol);
      case "haloalcano":
        if (CheckOtherClasses("haloalcano", mol)) return false;
        return CheckForHaloalcanes(mol);
      case "amina":
        if (CheckOtherClasses("amina", mol)) return false;
        return CheckForAmines(mol);
      case "cetona":
        if (CheckOtherClasses("cetona", mol)) return false;
        return CheckForCetones(mol);
      case "acido carbo":
        if (CheckOtherClasses("acido carbo", mol)) return false;
        return CheckForCarboxylicAcids(mol);
      case "aldeido":
        if (CheckOtherClasses("aldeido", mol)) return false;
        return CheckForAldeides(mol);
    }
    return false;
  }

  public static bool CheckTheClass(string classe, string mol)
  {
    switch (classe)
    {
      case "alcool":
        return CheckForAlcohol(mol);
    }
    return false;
  }

  public static bool CheckForHidrocarbonates(GameObject molecule)
  {
    for (int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.CompareTag("Interactable"))
      {
        string type = child.GetComponent<Atom>().GetAtomType();
        if (type != "Carbon" && type != "Hidrogen")
          return false;
      }
    }
    return true;
  }

  public static bool CheckForHaloalcanes(GameObject molecule)
  {

    for (int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.CompareTag("Interactable"))
      {
        string type = child.GetComponent<Atom>().GetAtomType();
        if (type == "Fluorine" || type == "Chlorine" || type == "Bromine" || type == "Iodine")
          return true;
      }
    }
    return false;
  }

  public static bool CheckForAmines(GameObject molecule)
  {
    
    for (int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.CompareTag("Interactable"))
      {
        string type = child.GetComponent<Atom>().GetAtomType();
        if (type == "Nitrogen")
          return true;
      }
    }
    return false;
  }

  public static bool CheckForAldeides(GameObject molecule)
  {
    
    for (int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.CompareTag("Interactable") && child.GetComponent<Atom>().GetAtomType() == "Carbon")
      {
        List<GameObject> bonds = molecule.GetComponent<Molecule>().GetAllBonds(child.gameObject);
        int OBonds = 0;
        int CBonds = 0;
        int OAtoms = 0;
        for (int j = 0; j < bonds.Count; j++)
        {
          if (bonds[j].GetComponent<BondController>().HasTypeAtom("Oxigen"))
          {
            int bondType = bonds[j].GetComponent<BondController>().GetBondType();
            OBonds += bondType;
            OAtoms++;
          }
          else if (bonds[j].GetComponent<BondController>().HasDoubleTypeAtom("Carbon"))
          {
            int bondType = bonds[j].GetComponent<BondController>().GetBondType();
            CBonds += bondType;
          }
        }
        if (OBonds == 2 && OAtoms == 1 && CBonds <= 1)
        {
          return true;
        }
      }
    }
    return false;
  }

  public static bool CheckForAlcohol(string moleculeStruc)
  {
    string[] split = moleculeStruc.Split('_');
    for (int i = 0; i < split.Length; i++)
    {
      string bond = split[i].Trim();
      if (bond == "Oxigen-1-Hidrogen" || bond == "Hidrogen-1-Oxigen") return true;
    }
    return false;
  }

  public static bool CheckForCetones(GameObject molecule)
  {
    for (int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.CompareTag("Interactable") && child.GetComponent<Atom>().GetAtomType() == "Carbon")
      {
        List<GameObject> bonds = molecule.GetComponent<Molecule>().GetAllBonds(child.gameObject);
        bool OBond = false;
        int CBonds = 0;
        for (int j = 0; j < bonds.Count; j++)
        {
          if (bonds[j].GetComponent<BondController>().HasTypeAtom("Oxigen"))
          {
            int bondType = bonds[j].GetComponent<BondController>().GetBondType();
            if (bondType == 2) OBond = true;
          }
          if (bonds[j].GetComponent<BondController>().HasTypeAtom("Carbon"))
          {
            int bondType = bonds[j].GetComponent<BondController>().GetBondType();
            CBonds += bondType;
          }
        }
        if (OBond && CBonds > 2)
        {
          return true;
        }
      }
    }
    return false;
  }

  public static bool CheckForCarboxylicAcids(GameObject molecule)
  {
    for(int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if(child.CompareTag("Interactable") && child.GetComponent<Atom>().GetAtomType() == "Carbon")
      {
        List<GameObject> bonds = molecule.GetComponent<Molecule>().GetAllBonds(child.gameObject);
        int OBonds = 0;
        bool hasAlcohol = false;
        for(int j = 0; j < bonds.Count; j++)
        {
          if (bonds[j].GetComponent<BondController>().HasTypeAtom("Oxigen"))
          {
            int bondType = bonds[j].GetComponent<BondController>().GetBondType();
            OBonds += bondType;
            if (bondType == 1)
              hasAlcohol = CheckForAlcoholInCA(bonds[j]);
          }
        }
        if(OBonds == 3 && hasAlcohol)
        {
          return true;
        }
      }
    }
    return false;
  }

  private static bool CheckForAlcoholInCA(GameObject bond)
  {
    Transform[] atoms = bond.GetComponent<BondController>().GetAtoms();
    Transform OAtom = atoms[0];
    if(atoms[1].GetComponent<Atom>().GetAtomType() == "Oxigen")
    {
      OAtom = atoms[1];
    }
    List<GameObject> bonds = OAtom.parent.GetComponent<Molecule>().GetAllBonds(OAtom.gameObject);
    for(int i = 0; i < bonds.Count; i++)
    {
      if (bonds[i].GetComponent<BondController>().HasTypeAtom("Hidrogen"))
        return true;
    }
    return false;
  }

  private static bool CheckOtherClasses(string wantedClass, GameObject mol)
  {
    switch (wantedClass)
    {
      case "hidrocarbonato":
        if (CheckForHaloalcanes(mol) || CheckForAmines(mol) || CheckForCetones(mol) || CheckForCarboxylicAcids(mol) || CheckForAldeides(mol))
          return true;
        else return false;
      case "haloalcano":
        if (CheckForHidrocarbonates(mol) || CheckForAmines(mol) || CheckForCetones(mol) || CheckForCarboxylicAcids(mol) || CheckForAldeides(mol))
          return true;
        else return false;
      case "amina":
        if (CheckForHaloalcanes(mol) || CheckForHidrocarbonates(mol) || CheckForCetones(mol) || CheckForCarboxylicAcids(mol) || CheckForAldeides(mol))
          return true;
        else return false;
      case "cetona":
        if (CheckForHaloalcanes(mol) || CheckForAmines(mol) || CheckForHidrocarbonates(mol) || CheckForCarboxylicAcids(mol) || CheckForAldeides(mol))
          return true;
        else return false;
      case "acido carbo":
        if (CheckForHaloalcanes(mol) || CheckForAmines(mol) || CheckForCetones(mol) || CheckForHidrocarbonates(mol) || CheckForAldeides(mol))
          return true;
        else return false;
      case "aldeido":
        if (CheckForHaloalcanes(mol) || CheckForAmines(mol) || CheckForCetones(mol) || CheckForCarboxylicAcids(mol) || CheckForHidrocarbonates(mol))
          return true;
        else return false;
    }
    return false;
  }

}
