using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoleculesCharacteristics {

  public static bool CheckTheClass(string classe, GameObject mol)
  {
    switch (classe)
    {
      case "hidrocarbonatos":
        return CheckForHidrocarbonates(mol);
      case "halocalnos":
        return CheckForHaloalcanes(mol);
      case "aminas":
        return CheckForAmines(mol);
      case "cetonas":
        return CheckForCetones(mol);
      case "acidos carbo":
        return CheckForCarboxylicAcids(mol);
    }
    return false;
  }

  public static bool CheckTheClass(string classe, string mol)
  {
    switch (classe)
    {
      case "aldeidos":
        return CheckForAldeides(mol);
      case "alcool":
        return CheckForAlcohol(mol);
    }
    return false;
  }

  public static bool CheckForHidrocarbonates(GameObject molecule)
  {
    for(int i = 0; i < molecule.transform.childCount; i++)
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
        if (type == "Fluorine" && type == "Chlorine" && type == "Bromine" && type == "Iodine")
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

  public static bool CheckForAldeides(string moleculeStruc)
  {
    string[] split = moleculeStruc.Split('_');
    bool OBond = false;
    bool HBond = false;
    for(int i = 0; i < split.Length; i++)
    {
      string bond = split[i].Trim();
      if (bond == "Carbon-2-Oxigen") OBond = true;
      if (OBond && bond == "Carbon-1-Hidrogen") return true;


      if (bond == "Carbon-1-Hidrogen") HBond = true;
      if (HBond && bond == "Carbon-2-Oxigen") return true;
    }
    return false;
  }

  public static bool CheckForAlcohol(string moleculeStruc)
  {
    string[] split = moleculeStruc.Split('_');
    for (int i = 0; i < split.Length; i++)
    {
      string bond = split[i].Trim();
      if (bond == "Oxigen-1-Hidrogen") return true;
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
            if (bondType == 2)
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

}
