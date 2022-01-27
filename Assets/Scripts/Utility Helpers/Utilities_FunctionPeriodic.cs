using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    // Executes a Function periodically
    public class Utilities_FunctionPeriodic
    {
        private GameObject gameObject;
        private float timer;
        private float baseTimer;
        private bool useUnscaledDeltaTime;
        private string functionName;
        public Action action;
        public Func<bool> testDestroy;

        // Holds a reference to all active timers
        private static List<Utilities_FunctionPeriodic> funcList;
        // Global game object used for initializing class, is destroyed on scene change
        private static GameObject initGameObject;


        //Class to hook Actions into MonoBehaviour
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
                initGameObject = new GameObject("FunctionPeriodic_Global");
                funcList = new List<Utilities_FunctionPeriodic>();
            }
        }

        // Persist through scene loads
        public static Utilities_FunctionPeriodic Create_Global(Action action, Func<bool> testDestroy, float timer) {
            Utilities_FunctionPeriodic functionPeriodic = Create(action, testDestroy, timer, "", false, false, false);
            MonoBehaviour.DontDestroyOnLoad(functionPeriodic.gameObject);
            return functionPeriodic;
        }

        // Trigger [action] every [timer], execute [testDestroy] after triggering action, destroy if returns true
        public static Utilities_FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer) =>
            Create(action, testDestroy, timer, "", false);

        public static Utilities_FunctionPeriodic Create(Action action, float timer) =>
            Create(action, null, timer, "", false, false, false);

        public static Utilities_FunctionPeriodic Create(Action action, float timer, string functionName) =>
            Create(action, null, timer, functionName, false, false, false);

        public static Utilities_FunctionPeriodic Create(Action callback, Func<bool> testDestroy,
            float timer, string functionName, bool stopAllWithSameName) =>
            Create(callback, testDestroy, timer, functionName, false, false, stopAllWithSameName);

        public static Utilities_FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer,
            string functionName, bool useUnscaledDeltaTime, bool triggerImmediately, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
                StopAllFunc(functionName);

            GameObject gameObject = new GameObject("FunctionPeriodic Object " + functionName, typeof(MonoBehaviourHook));
            Utilities_FunctionPeriodic functionPeriodic = new Utilities_FunctionPeriodic(gameObject, action, timer, testDestroy, functionName, useUnscaledDeltaTime);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionPeriodic.Update;

            funcList.Add(functionPeriodic);

            if (triggerImmediately)
                action();

            return functionPeriodic;
        }


        public static void RemoveTimer(Utilities_FunctionPeriodic funcTimer)
        {
            InitIfNeeded();
            funcList.Remove(funcTimer);
        }
        public static void StopTimer(string _name)
        {
            InitIfNeeded();

            for (int i = 0; i < funcList.Count; i++)
            {
                if (funcList[i].functionName == _name)
                {
                    funcList[i].DestroySelf();
                    return;
                }
            }
        }
        public static void StopAllFunc(string _name)
        {
            InitIfNeeded();

            for (int i = 0; i < funcList.Count; i++)
            {
                if (funcList[i].functionName == _name)
                {
                    funcList[i].DestroySelf();
                    i--;
                }
            }
        }
        public static bool IsFuncActive(string name)
        {
            InitIfNeeded();

            for (int i = 0; i < funcList.Count; i++)
            {
                if (funcList[i].functionName == name)
                    return true;
            }
            return false;
        }


        private Utilities_FunctionPeriodic(GameObject gameObject, Action action, float timer, Func<bool> testDestroy, string functionName, bool useUnscaledDeltaTime)
        {
            this.gameObject = gameObject;
            this.action = action;
            this.timer = timer;
            this.testDestroy = testDestroy;
            this.functionName = functionName;
            this.useUnscaledDeltaTime = useUnscaledDeltaTime;
            baseTimer = timer;
        }

        public void SkipTimerTo(float timer) => this.timer = timer;
        public void SetBaseTimer(float baseTimer) => this.baseTimer = baseTimer;
        public float GetBaseTimer() => baseTimer;

        private void Update()
        {
            if (useUnscaledDeltaTime)
                timer -= Time.unscaledDeltaTime;
            else
                timer -= Time.deltaTime;
            if (timer <= 0)
            {
                action();

                if (testDestroy != null && testDestroy())
                    //Destroy
                    DestroySelf();
                else
                    //Repeat
                    timer += baseTimer;
            }
        }

        public void DestroySelf()
        {
            RemoveTimer(this);
            if (gameObject != null)
                UnityEngine.Object.Destroy(gameObject);
        }
    }
}