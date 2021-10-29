using System;
using UnityEngine;

namespace TargetTimerDemo
{

    public class Timer : MonoBehaviour
    {
        public static Action TimerComplete;

        private float timeLeft;
        private float timeElapsed;
        private bool isPaused;
        private bool isFinished;
        public float Duration { get; set; }

        public TextMesh DisplayTime;

        public float startTime { get; set; }
        public float finishTime { get; set; }
        private bool useTimer = false;
        private bool showTime = false;

        protected virtual void Start()
        {
            //if (DisplayTime != null)
                //showTime = true;
        }

        protected virtual void Update()
        {
            if (!useTimer) return;
            if (isPaused) return;

            timeLeft = finishTime - Time.time;
            timeElapsed = Duration - timeLeft;

            if (Time.time > finishTime)
            {
                TimeOver();
                return;
            }

            if (showTime)
            {
                //timeLeft = finishTime - Time.time;
                //timeElapsed = Duration - timeLeft;
                string minutes = Mathf.Floor(timeLeft / 60).ToString("00");
                string seconds = (timeLeft % 60).ToString("00");
                DisplayTime.text = $"{minutes}:{seconds}";
                DisplayTime.color = Color.red;
            }
        }

        public void StartCount()
        {
            if (DisplayTime != null)
                showTime = true;
        }


        private void TimeOver()
        {
            isFinished = true;
            useTimer = false;
            TimerComplete?.Invoke();
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Disable()
        {
            useTimer = false;
        }

        public void Set(float length)
        {
            Duration = length;
            startTime = Time.time;
            finishTime = startTime + Duration;
            timeLeft = startTime - Time.time;
            timeElapsed = Duration - timeLeft;
            isFinished = false;
            isPaused = false;
            useTimer = true;
        }

        public void UnPause()
        {
            isPaused = false;
        }

    }
}

