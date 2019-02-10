using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    //[Header("Real Time cycle length")]
    //[SerializeField][Range(0,23)] int hours;
    //[SerializeField][Range(0,59)] int minutes;
    //[SerializeField][Range(0,59)] int seconds;

    //float fSec;
    //float curTime;
    //float percent;

    //private void Start()
    //{
    //    fSec = (hours * 60 * 60) + (minutes * 60) + seconds;
    //    StartCoroutine(KeepTime());
    //}

    //IEnumerator KeepTime()
    //{
    //    while (true)
    //    {
    //        curTime += Time.deltaTime;

    //        curTime = curTime > fSec ? 0 : curTime;

    //        percent = curTime / fSec;

    //        yield return null;
    //    }
    //}
}
