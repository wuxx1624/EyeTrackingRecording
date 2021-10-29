/*
 * Asynchronous CSV Writer
 * Jerald 06/18/2019
 */

using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class CSVLogger2 : MonoBehaviour {

    private MazeVRTP2 experiment_control;


    [Tooltip("Log directory relative to the project directory")]
    private string logDirectory = string.Empty;

    [Tooltip("Log name")]
    private string logName = "log";

    [Tooltip("What field values default to")]
    private string defaultValue = "N/A";

    [Tooltip("Character that separates values")]
    private char deliminator = ',';

    [Tooltip("If true, all values will enqueue to the write queue on LateUpdate")]
    public bool queueOnLateupdate = false;

    [Tooltip("If true, all values will be set to defaultValue after queueing")]
    public bool flushOnQueue = true;

    [Tooltip("List of fields. Do not modify after Start")]
    private List<string> fields = new List<string>();

    private Dictionary<string, string> values = new Dictionary<string, string>();

    private Thread writeThread;
    private readonly object writeLock = new object();
    private volatile Queue<string> writeQueue = new Queue<string>();
    private volatile bool threadRunning = true;
    private volatile StreamWriter writer;
    
	void Awake ()
    {
        /*
        writeThread = new Thread(new ThreadStart(ThreadJob));
        writeThread.Start();*/
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


    void OnApplicationQuit()
    {
        lock (writeLock)
        {
            threadRunning = false;
            writer.Close();
        }
    }

    public void Initialization(string directionary, string name, char deli, List<string> field)
    {
        logDirectory = directionary;
        logName = name;
        deliminator = deli;
        fields = field;

        string header = "";

        foreach (string child in fields)
        {
            values.Add(child, defaultValue);
            header += child + deliminator;
        }

        writeQueue.Enqueue(header);

        if (logDirectory == string.Empty)
            writer = new StreamWriter(logName);
        else if (!Directory.Exists(logDirectory))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(logDirectory);
        }
        else
            writer = new StreamWriter(logDirectory + logName);
        writeThread = new Thread(new ThreadStart(ThreadJob));
        writeThread.Start();
    }

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
