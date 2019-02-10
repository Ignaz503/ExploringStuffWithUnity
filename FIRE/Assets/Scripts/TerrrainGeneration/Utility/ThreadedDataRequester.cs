using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;

public class ThreadedDataRequester : MonoBehaviour {

    public static ThreadedDataRequester instacne;

    private void Awake()
    {
        instacne = FindObjectOfType<ThreadedDataRequester>();
    }

#if UNITY_EDITOR

    public void MakeUseableInEditor()
    {
        instacne = this;
        EditorApplication.update += Update;
        EditorApplication.quitting += OnQuit;
    }

    private void OnQuit()
    {
        EditorApplication.update -= Update;
    }

#endif

    Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        ThreadStart tStart = delegate {
            instacne.DataThread(generateData, callback);
        };
        Thread t = new Thread(tStart) { IsBackground = true };
        t.Start();
    }

    void DataThread(Func<object> generateData, Action<object> callback)
    {

        object data = generateData();

        lock (dataQueue)
        {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

    private void Update()
    {
        HandleDataQueue();
    }

    void HandleDataQueue()
    {
        if (dataQueue.Count > 0)
        {
            for (int i = 0; i < dataQueue.Count; i++)
            {
                ThreadInfo threadInfo = dataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    struct ThreadInfo
    {
        public readonly Action<object> callback;
        public readonly object  parameter;

        public ThreadInfo(Action<object> callback, object parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

}
