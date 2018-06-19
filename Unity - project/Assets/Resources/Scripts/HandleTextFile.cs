using UnityEngine;
using UnityEditor;
using System.IO;

public class HandleTextFile
{
  private static string path = "Assets/Resources/SavedFiles/";

  public static void SaveFile(string name, string text)
  {

    string finalPath = path + name;

    //Write some text to the test.txt file
    StreamWriter writer = new StreamWriter(finalPath, true);

    writer.WriteLine(text);
    writer.Close();

    //Re-import the file to update the reference in the editor
    AssetDatabase.ImportAsset(finalPath);
    TextAsset asset = Resources.Load(name) as TextAsset;

    //Print the text from the file
    //Debug.Log(asset.text);
  }
  
  public static string ReadString(string name)
  {
    string finalPath = path + name;
    string text = "";
    //Read the text from directly from the test.txt file
    StreamReader reader = new StreamReader(finalPath);
    text = reader.ReadToEnd();
    reader.Close();
    return text;
  }

  public static void ClearFile(string name)
  {
    File.WriteAllText(path+name,"");
  }

  public static string ReadLevels(string name)
  {
    string finalPath = path + "Levels/" + name + ".txt";
    string text = "";
    //Read the text from directly from the test.txt file
    StreamReader reader = new StreamReader(finalPath);
    text = reader.ReadToEnd();
    reader.Close();
    return text;
  }

}
