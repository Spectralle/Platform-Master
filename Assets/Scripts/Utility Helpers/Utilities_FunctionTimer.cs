using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    // Triggers a Action after a certain time 
    public class Utilities_FunctionTimer
    {
        private GameObject gameObject;
        private float timer;
        private string functionName;
        private bool active;
        private bool useUnscaledDeltaTime;
        private Action action;

        // Holds a reference to all active timers
        private static List<Utilities_FunctionTimer> timerList;
        // Global game object used for initializing class, is destroyed on scene change
        private static GameObject initGameObject;
        
        
        // Class to hook Actions into MonoBehaviour
        private class MonoBehaviourHook : MonoBehaviour
        {
            public Action OnUpdate;

            private void Update()
            {
                if (OnUpdate != null)
                    OnUpdate();
            }

        }

        private static void InitIfNeeded()
        {
            if (initGameObject == null)
            {
                initGameObject = new GameObject("FunctionTimer_Global");
                timerList = new List<Utilities_FunctionTimer>();
            }
        }


        public static Utilities_FunctionTimer Create(Action action, float timer) =>
            Create(action, timer, "", false, false);

        public static Utilities_FunctionTimer Create(Action action, float timer, string functionName) =>
            Create(action, timer, functionName, false, false);

        public static Utilities_FunctionTimer Create(Action action, float timer, string functionName, bool useUnscaledDeltaTime) =>
            Create(action, timer, functionName, useUnscaledDeltaTime, false);

        public static Utilities_FunctionTimer Create(Action action, float timer, string functionName,
            bool useUnscaledDeltaTime, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
                StopAllTimersWithName(functionName);

            GameObject obj = new GameObject("FunctionTimer Object " + functionName, typeof(MonoBehaviourHook));
            Utilities_FunctionTimer funcTimer = new Utilities_FunctionTimer(obj, action, timer, functionName, useUnscaledDeltaTime);
            obj.GetComponent<MonoBehaviourHook>().OnUpdate = funcTimer.Update;

            timerList.Add(funcTimer);

            return funcTimer;
        }

        public static void RemoveTimer(Utilities_FunctionTimer funcTimer)
        {
            InitIfNeeded();
            timerList.Remove(funcTimer);
        }

        public static void StopAllTimersWithName(string functionName)
        {
            InitIfNeeded();
            for (int i = 0; i < timerList.Count; i++)
            {
                if (timerList[i].functionName == functionName)
                {
                    timerList[i].DestroySelf();
                    i--;
                }
            }
        }

        public static void StopFirstTimerWithName(string functionName)
        {
            InitIfNeeded();
            for (int i = 0; i < timerList.Count; i++)
            {
                if (timerList[i].functionName == functionName)
                {
                    timerList[i].DestroySelf();
                    return;
                }
            }
        }


        public Utilities_FunctionTimer(GameObject gameObject, Action action, float timer,
            string functionName, bool useUnscaledDeltaTime)
        {
            this.gameObject = gameObject;
            this.action = action;
            this.timer = timer;
            this.functionName = functionName;
            this.useUnscaledDeltaTime = useUnscaledDeltaTime;
        }

        private void Update()
        {
            if (useUnscaledDeltaTime)
                timer -= Time.unscaledDeltaTime;
            else
                timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // Timer complete, trigger Action
                action();
                DestroySelf();
            }
        }

        private void DestroySelf()
        {
            RemoveTimer(this);
            if (gameObject != null)
                UnityEngine.Object.Destroy(gameObject);
        }


        // Class to trigger Actions manually without creating a GameObject
        public class FunctionTimerObject
        {
            private float timer;
            private Action callback;

            public FunctionTimerObject(Action callback, float timer)
            {
                this.callback = callback;
                this.timer = timer;
            }

            public bool Update() => Update(Time.deltaTime);

            public bool Update(float deltaTime)
            {
                timer -= deltaTime;
                if (timer <= 0)
                {
                    callback();
                    return true;
                }
                else
                    return false;
            }
        }

        // Create a Object that must be manually updated through Update();
        public static FunctionTimerObject CreateObject(Action callback, float timer) =>
            new FunctionTimerObject(callback, timer);
    }
}