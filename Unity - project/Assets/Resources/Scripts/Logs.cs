using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Logs : MonoBehaviour {

  private static string path = "Assets/Resources/SavedFiles/";
  private static string task = "";
  private static int reloads = 0;

  private static string level = "";

  public static bool AddMethodToFile(string name, string text)
  {
    string s = ReadString(name);
    if (s == "") return false;

    string finalPath = path + name;

    //Write some text to the test.txt file
    StreamWriter writer = new StreamWriter(finalPath, true);
    string toWrite = "";
    toWrite += "reload: " + reloads + "\n";
    toWrite += "end time: " + Time.realtimeSinceStartup + "\n" + "\n";
    toWrite += text;
    reloads = 0;
    task = "";
    writer.WriteLine(toWrite);
    writer.Close();

    //Re-import the file to update the reference in the editor
    AssetDatabase.ImportAsset(finalPath);
    TextAsset asset = Resources.Load(name) as TextAsset;

    return true;
  }

  public static bool AddTaskToFile(string name, string text)
  {
    string s = ReadString(name);
    if (s == "") return false;

    string finalPath = path + name;
    
    //Write some text to the test.txt file
    string toWrite = "";
    if (task == "")
    {
      StreamWriter writer = new StreamWriter(finalPath, true);
      toWrite += "new task " + text + "\n";
      toWrite += "start time: " + Time.realtimeSinceStartup;
      writer.WriteLine(toWrite);
      writer.Close();
      AssetDatabase.ImportAsset(finalPath);
      TextAsset asset = Resources.Load(name) as TextAsset;
      Debug.Log("start");
    }
    else if (task != text && task != "")
    {
      StreamWriter writer = new StreamWriter(finalPath, true);
      toWrite += "reload: " + reloads + "\n";
      toWrite += "end time: " + Time.realtimeSinceStartup + "\n" + "\n";
      toWrite += "new task " + text + "\n";
      toWrite += "start time: " + Time.realtimeSinceStartup;
      writer.WriteLine(toWrite);
      writer.Close();
      reloads = 0;
      AssetDatabase.ImportAsset(finalPath);
      TextAsset asset = Resources.Load(name) as TextAsset;
      Debug.Log("end");
    }
    else if(task == text)
    {
      reloads++;
      Debug.Log("reloads");
    }
    task = text;
    //Re-import the file to update the reference in the editor


    return true;
  }

  public static void SaveFile(string name, string text)
  {
    string s = ReadString(name);
    if (s != "") AddMethodToFile(name, text);

    else
    {
      string finalPath = path + name;

      //Write some text to the test.txt file
      StreamWriter writer = new StreamWriter(finalPath, true);

      writer.WriteLine(text);
      writer.Close();

      //Re-import the file to update the reference in the editor
      AssetDatabase.ImportAsset(finalPath);
      TextAsset asset = Resources.Load(name) as TextAsset;
    }
  }

  public static string ReadString(string name)
  {
    string finalPath = path + name;
    string text = "";
    //Read the text from directly from the test.txt file
    try
    {
      StreamReader reader = new StreamReader(finalPath);
      text = reader.ReadToEnd();
      reader.Close();
    }catch(Exception e)
    {

    }
    return text;
  }

  public static void ClearFile(string name)
  {
    File.WriteAllText(path + name, "");
  }


  public static void BeginFile(string name, string[] info)
  {
    string finalPath = path + name;

    //Write some text to the test.txt file
    StreamWriter writer = new StreamWriter(finalPath, true);
    string toWrite = "";
    toWrite += "Level: " + info[0] + "\n";
    toWrite += "Task: " + info[1] + "\n";
    writer.WriteLine(toWrite);
    writer.Close();
    //Re-import the file to update the reference in the editor
    AssetDatabase.ImportAsset(finalPath);
    TextAsset asset = Resources.Load(name) as TextAsset;
  }

  public static bool EndLevel(string name, string[] info)
  {
    string s = ReadString(name);
    if (s == "") return false;

    string finalPath = path + name;

    //Write some text to the test.txt file
    StreamWriter writer = new StreamWriter(finalPath, true);
    string toWrite = "";
    toWrite += "Moves: " + info[2] + "\n";
    toWrite += "Time: " + info[3] + "\n";
    toWrite += "---------------------------------------" + "\n";
    toWrite += "Level: " + info[0]+"\n";
    toWrite += "Task: " + info[1] + "\n";
    level = info[0];
    task = info[1];
    writer.WriteLine(toWrite);
    writer.Close();

    //Re-import the file to update the reference in the editor
    AssetDatabase.ImportAsset(finalPath);
    TextAsset asset = Resources.Load(name) as TextAsset;

    return true;
  }

  public static bool EndTask(string name, string[] info)
  {
    string s = ReadString(name);
    if (s == "") return false;

    string finalPath = path + name;

    //Write some text to the test.txt file
    string toWrite = "";

      StreamWriter writer = new StreamWriter(finalPath, true);
      toWrite += "Moves: " + info[2] + "\n";
      toWrite += "Time: " + info[3] + "\n";
      toWrite += "Task " + info[1] + "\n";
      writer.WriteLine(toWrite);
      writer.Close();
      AssetDatabase.ImportAsset(finalPath);
      TextAsset asset = Resources.Load(name) as TextAsset;
    //Re-import the file to update the reference in the editor


    return true;
  }

}
