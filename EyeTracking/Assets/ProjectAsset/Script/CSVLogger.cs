/*
 * Asynchronous CSV Writer
 * Jerald 06/18/2019
 */

using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class CSVLogger : MonoBehaviour
{


    [Tooltip("Log directory relative to the project directory")]
    public string logDirectory = string.Empty;

    [Tooltip("Log name")]
    public string logName = "log";

    [Tooltip("What field values default to")]
    public string defaultValue = "N/A";

    [Tooltip("Character that separates values")]
    public char deliminator = ',';

    [Tooltip("If true, all values will enqueue to the write queue on LateUpdate")]
    public bool queueOnLateupdate = false;

    [Tooltip("If true, all values will be set to defaultValue after queueing")]
    public bool flushOnQueue = true;

    [Tooltip("List of fields. Do not modify after Start")]
    public List<string> fields = new List<string>();

    private Dictionary<string, string> values = new Dictionary<string, string>();

    private Thread writeThread;
    private readonly object writeLock = new object();
    private volatile Queue<string> writeQueue = new Queue<string>();
    private volatile bool threadRunning = true;
    private volatile StreamWriter writer;

    private GlobalStatistics globalData;

    void Awake()
    {
        
    }


    public void Initial()
    {
        globalData = GlobalControl.Instance.savedData;

        string header = "";

        foreach (string field in fields)
        {
            values.Add(field, defaultValue);
            header += field + deliminator;
        }

        writeQueue.Enqueue(header);

        if (globalData.current_condition != null)
            logName = globalData.current_condition + "_" + logName;
        if (globalData.user_id != null)
            logName = globalData.user_id + "_" + logName;
        if (logDirectory == string.Empty)
            logDirectory = Application.dataPath;
        else
            logDirectory = Application.dataPath + "/" + logDirectory;
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
        writer = new StreamWriter(logDirectory + "/" + logName);
        writeThread = new Thread(new ThreadStart(ThreadJob));
        writeThread.Start();
    }

    void LateUpdate()
    {
        if (queueOnLateupdate)
            Queue();
    }

    private void OnDestroy()
    {
        lock (writeLock)
        {
            threadRunning = false;
            writer.Close();
        }
    }

    public void close()
    {
        lock (writeLock)
        {
            threadRunning = false;
            writer.Close();
        }
    }

    /*
    void OnApplicationQuit()
    {
        lock (writeLock)
        {
            threadRunning = false;
            writer.Close();
        }
    }
    */
    public void UpdateField(string field, string value)
    {
        if (fields.Contains(field))
        {
            values[field] = value;
        }
        else
        {
            Debug.LogWarning("CSVLogger: Log <" + logName + "> does not contain field <" + field + ">.");
        }
    }

    public void Queue()
    {
        string line = "";
        foreach (string field in fields)
        {
            line += values[field] + deliminator;
        }

        lock (writeLock)
        {
            writeQueue.Enqueue(line);
        }

        if (flushOnQueue)
            Flush();
    }

    public void Flush()
    {
        foreach (string field in fields)
        {
            values[field] = defaultValue;
        }
    }

    void ThreadJob()
    {
        while (threadRunning)
        {
            while (writeQueue.Count > 0)
            {
                lock (writeLock)
                {
                    writer.WriteLine(writeQueue.Dequeue());
                }
            }
        }
    }


}
