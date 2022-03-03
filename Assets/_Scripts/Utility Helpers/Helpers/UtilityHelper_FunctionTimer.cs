using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    // Triggers a Action after a certain time 
    public class UtilityHelper_FunctionTimer
    {
        private GameObject _gameObject;
        private float _timer;
        private string _functionName;
        private bool _active;
        private bool _useUnscaledDeltaTime;
        private Action _action;

        // Holds a reference to all active timers
        private static List<UtilityHelper_FunctionTimer> _timerList;
        // Global game object used for initializing class, is destroyed on scene change
        private static GameObject _initGameObject;
        
        
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
            if (_initGameObject == null)
            {
                _initGameObject = new GameObject("FunctionTimer_Global");
                _timerList = new List<UtilityHelper_FunctionTimer>();
            }
        }


        public static UtilityHelper_FunctionTimer Create(Action action, float timer) =>
            Create(action, timer, "", false, false);

        public static UtilityHelper_FunctionTimer Create(Action action, float timer, string functionName) =>
            Create(action, timer, functionName, false, false);

        public static UtilityHelper_FunctionTimer Create(Action action, float timer, string functionName, bool useUnscaledDeltaTime) =>
            Create(action, timer, functionName, useUnscaledDeltaTime, false);

        public static UtilityHelper_FunctionTimer Create(Action action, float timer, string functionName,
            bool useUnscaledDeltaTime, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
                StopAllTimersWithName(functionName);

            GameObject obj = new GameObject("FunctionTimer Object " + functionName, typeof(MonoBehaviourHook));
            UtilityHelper_FunctionTimer funcTimer = new UtilityHelper_FunctionTimer(obj, action, timer, functionName, useUnscaledDeltaTime);
            obj.GetComponent<MonoBehaviourHook>().OnUpdate = funcTimer.Update;

            _timerList.Add(funcTimer);

            return funcTimer;
        }

        public static void RemoveTimer(UtilityHelper_FunctionTimer funcTimer)
        {
            InitIfNeeded();
            _timerList.Remove(funcTimer);
        }

        public static void StopAllTimersWithName(string functionName)
        {
            InitIfNeeded();
            for (int i = 0; i < _timerList.Count; i++)
            {
                if (_timerList[i]._functionName == functionName)
                {
                    _timerList[i].DestroySelf();
                    i--;
                }
            }
        }

        public static void StopFirstTimerWithName(string functionName)
        {
            InitIfNeeded();
            for (int i = 0; i < _timerList.Count; i++)
            {
                if (_timerList[i]._functionName == functionName)
                {
                    _timerList[i].DestroySelf();
                    return;
                }
            }
        }


        public UtilityHelper_FunctionTimer(GameObject gameObject, Action action, float timer,
            string functionName, bool useUnscaledDeltaTime)
        {
            this._gameObject = gameObject;
            this._action = action;
            this._timer = timer;
            this._functionName = functionName;
            this._useUnscaledDeltaTime = useUnscaledDeltaTime;
        }

        private void Update()
        {
            if (_useUnscaledDeltaTime)
                _timer -= Time.unscaledDeltaTime;
            else
                _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                // Timer complete, trigger Action
                _action();
                DestroySelf();
            }
        }

        private void DestroySelf()
        {
            RemoveTimer(this);
            if (_gameObject != null)
                UnityEngine.Object.Destroy(_gameObject);
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