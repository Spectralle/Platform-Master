using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    // Executes a Function periodically
    public class UtilityHelper_FunctionPeriodic
    {
        private GameObject _gameObject;
        private float _timer;
        private float _baseTimer;
        private bool _useUnscaledDeltaTime;
        private string _functionName;
        public Action Action;
        public Func<bool> TestDestroy;

        // Holds a reference to all active timers
        private static List<UtilityHelper_FunctionPeriodic> _funcList;
        // Global game object used for initializing class, is destroyed on scene change
        private static GameObject _initGameObject;


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
            if (_initGameObject == null)
            {
                _initGameObject = new GameObject("FunctionPeriodic_Global");
                _funcList = new List<UtilityHelper_FunctionPeriodic>();
            }
        }

        // Persist through scene loads
        public static UtilityHelper_FunctionPeriodic Create_Global(Action action, Func<bool> testDestroy, float timer) {
            UtilityHelper_FunctionPeriodic functionPeriodic = Create(action, testDestroy, timer, "", false, false, false);
            MonoBehaviour.DontDestroyOnLoad(functionPeriodic._gameObject);
            return functionPeriodic;
        }

        // Trigger [action] every [timer], execute [testDestroy] after triggering action, destroy if returns true
        public static UtilityHelper_FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer) =>
            Create(action, testDestroy, timer, "", false);

        public static UtilityHelper_FunctionPeriodic Create(Action action, float timer) =>
            Create(action, null, timer, "", false, false, false);

        public static UtilityHelper_FunctionPeriodic Create(Action action, float timer, string functionName) =>
            Create(action, null, timer, functionName, false, false, false);

        public static UtilityHelper_FunctionPeriodic Create(Action callback, Func<bool> testDestroy,
            float timer, string functionName, bool stopAllWithSameName) =>
            Create(callback, testDestroy, timer, functionName, false, false, stopAllWithSameName);

        public static UtilityHelper_FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer,
            string functionName, bool useUnscaledDeltaTime, bool triggerImmediately, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
                StopAllFunc(functionName);

            GameObject gameObject = new GameObject("FunctionPeriodic Object " + functionName, typeof(MonoBehaviourHook));
            UtilityHelper_FunctionPeriodic functionPeriodic = new UtilityHelper_FunctionPeriodic(gameObject, action, timer, testDestroy, functionName, useUnscaledDeltaTime);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionPeriodic.Update;

            _funcList.Add(functionPeriodic);

            if (triggerImmediately)
                action();

            return functionPeriodic;
        }


        public static void RemoveTimer(UtilityHelper_FunctionPeriodic funcTimer)
        {
            InitIfNeeded();
            _funcList.Remove(funcTimer);
        }
        public static void StopTimer(string _name)
        {
            InitIfNeeded();

            for (int i = 0; i < _funcList.Count; i++)
            {
                if (_funcList[i]._functionName == _name)
                {
                    _funcList[i].DestroySelf();
                    return;
                }
            }
        }
        public static void StopAllFunc(string _name)
        {
            InitIfNeeded();

            for (int i = 0; i < _funcList.Count; i++)
            {
                if (_funcList[i]._functionName == _name)
                {
                    _funcList[i].DestroySelf();
                    i--;
                }
            }
        }
        public static bool IsFuncActive(string name)
        {
            InitIfNeeded();

            for (int i = 0; i < _funcList.Count; i++)
            {
                if (_funcList[i]._functionName == name)
                    return true;
            }
            return false;
        }


        private UtilityHelper_FunctionPeriodic(GameObject gameObject, Action action, float timer, Func<bool> testDestroy, string functionName, bool useUnscaledDeltaTime)
        {
            this._gameObject = gameObject;
            this.Action = action;
            this._timer = timer;
            this.TestDestroy = testDestroy;
            this._functionName = functionName;
            this._useUnscaledDeltaTime = useUnscaledDeltaTime;
            _baseTimer = timer;
        }

        public void SkipTimerTo(float timer) => this._timer = timer;
        public void SetBaseTimer(float baseTimer) => this._baseTimer = baseTimer;
        public float GetBaseTimer() => _baseTimer;

        private void Update()
        {
            if (_useUnscaledDeltaTime)
                _timer -= Time.unscaledDeltaTime;
            else
                _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                Action();

                if (TestDestroy != null && TestDestroy())
                    //Destroy
                    DestroySelf();
                else
                    //Repeat
                    _timer += _baseTimer;
            }
        }

        public void DestroySelf()
        {
            RemoveTimer(this);
            if (_gameObject != null)
                UnityEngine.Object.Destroy(_gameObject);
        }
    }
}