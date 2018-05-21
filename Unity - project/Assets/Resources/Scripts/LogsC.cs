using System.IO;
using UnityEngine;
using LitJson;
using System.Collections.Generic;

public class LogsC : MonoBehaviour
{

  public static LogsC Instance;
  private JsonData logData = null;
  private string fileName = "log_lll.json";

  private JsonData session = null;


  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    DontDestroyOnLoad(Instance);

    logData = JsonMapper.ToObject(readFile(getSaveDir(), fileName));
    Debug.Log(getSaveDir());
  }


  void OnApplicationQuit()
  {
    endSession();
  }


  public void startSession(string taskName)
  {
    session = new JsonData();

    session["data"] = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    session["time"] = Time.realtimeSinceStartup;
    session["tasks"] = new JsonData();

    addNewTask(taskName);
  }

  public void endSession()
  {
    if (session == null)
      return;

    if (logData == null)
    {
      logData = new JsonData();
    }


    string tempo = session["time"].ToString();
    float time = float.Parse(session["time"].ToString());

    session["time"] = Time.realtimeSinceStartup - time;

    logData.Add(session);

    saveJsonFile(getSaveDir(), fileName, logData);
  }

  public void sessionNewTask(string taskName)
  {
    //int taskId = session["tasks"].Count - 1;
    //float time = float.Parse(session["tasks"][taskId]["time"].ToString());

    //session["tasks"][taskId]["time"] = Time.realtimeSinceStartup - time;
    
    addNewTask(taskName);
  }
  
  public void sessionStopTask ()
  {
    int taskId = session["tasks"].Count - 1;
    float time = float.Parse(session["tasks"][taskId]["time"].ToString());

    session["tasks"][taskId]["time"] = Time.realtimeSinceStartup - time;
  }

  #region tasks

  private void addNewTask(string taskName)
  {
    JsonData task = new JsonData();

    task["nome"] = taskName;
    task["time"] = Time.realtimeSinceStartup;
    task["subtasks"] = new JsonData();
    session["tasks"].Add(task);
    addNewSubTask(1);
  }
  #endregion

  #region subtasks
  private void addNewSubTask(int i)
  {
    JsonData subtask = new JsonData();

    subtask["id"] = i;
    subtask["time"] = Time.realtimeSinceStartup;
    subtask["cleanMolecule"] = 0;
    subtask["cleanAtoms"] = 0;
    subtask["reload"] = 0;
    subtask["distance"] = 0.0f;

    int taskId = session["tasks"].Count - 1;
    session["tasks"][taskId]["subtasks"].Add(subtask);
  }

  public void sessionStopSubTask ()
  {
    int taskId = session["tasks"].Count - 1;
    int subtaskId = session["tasks"][taskId]["subtasks"].Count - 1;
    float time = float.Parse(session["tasks"][taskId]["subtasks"][subtaskId]["time"].ToString());

    session["tasks"][taskId]["subtasks"][subtaskId]["time"] = Time.realtimeSinceStartup - time;
  }

  public void sessionSubTaskDistance (float dist)
  {
    if (session == null)
      return;

    int taskId = session["task"].Count - 1;
    int subtaskId = session["tasks"][taskId]["subtasks"].Count - 1;

    session["task"][taskId]["subtasks"][subtaskId]["distance"] = dist;
  }

  public void sessionSubTaskCleanMol()
  {
    if (session == null)
      return;

    int taskId = session["task"].Count - 1;
    int subtaskId = session["tasks"][taskId]["subtasks"].Count - 1;
    int nr = int.Parse(session["task"][taskId]["subtasks"][subtaskId]["cleanMolecule"].ToString());

    session["task"][taskId]["subtasks"][subtaskId]["cleanMolecule"] = nr + 1;
  }

  public void sessionSubTaskCleanAtom()
  {
    if (session == null)
      return;

    int taskId = session["tasks"].Count - 1;
    int subtaskId = session["tasks"][taskId]["subtasks"].Count - 1;
    int nr = int.Parse(session["tasks"][taskId]["subtasks"][subtaskId]["cleanAtoms"].ToString());

    session["tasks"][taskId]["subtasks"][subtaskId]["cleanAtoms"] = nr + 1;
  }

  public void sessionSubTaskReload()
  {
    if (session == null)
      return;

    int taskId = session["tasks"].Count - 1;
    int subtaskId = session["tasks"][taskId]["subtasks"].Count - 1;
    int nr = int.Parse(session["tasks"][taskId]["subtasks"][subtaskId]["reload"].ToString());

    session["tasks"][taskId]["subtasks"][subtaskId]["reload"] = nr + 1;
  }

  public void sessionNewSubTask()
  {
    int taskId = session["tasks"].Count - 1;
    int subtaskId = session["tasks"][taskId]["subtasks"].Count - 1;
    //float time = float.Parse(session["tasks"][taskId]["subtasks"][subtaskId]["time"].ToString());

    //session["tasks"][taskId]["subtasks"][subtaskId]["time"] = Time.realtimeSinceStartup - time;
    addNewSubTask(subtaskId+2);
  }
  #endregion subtasks



  #region fileUtils
  public string getSaveDir()
  {
    string realPath = Application.persistentDataPath;
    realPath = realPath.Substring(0, realPath.LastIndexOf("/"));
    realPath = realPath.Substring(0, realPath.LastIndexOf("/"));

    string confDir = realPath;

    Directory.CreateDirectory(confDir);

    return confDir;
  }

  public string readFile(string dir, string fileName)
  {

    string conf = Path.Combine(dir, fileName);

    if (File.Exists(conf))
    {
      using (StreamReader streamerRead = new StreamReader(conf))
      {
        return streamerRead.ReadToEnd();
      }
    }

    saveFile(dir, fileName, "");

    return "";
  }

  public void saveJsonFile(string dir, string fileName, JsonData data)
  {
    saveFile(dir, fileName, data.ToJson());
  }

  public void saveFile(string dir, string fileName, string data)
  {
    string conf = Path.Combine(dir, fileName);

    using (var writer = new StreamWriter(conf))
    {
      writer.WriteLine(data);
    }
  }
  #endregion
}
