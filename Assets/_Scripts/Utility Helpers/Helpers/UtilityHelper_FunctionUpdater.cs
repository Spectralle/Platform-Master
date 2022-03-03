using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    // Calls function on every Update until it returns true
    public class UtilityHelper_FunctionUpdater
    {
        private GameObject _gameObject;
        private string _functionName;
        private bool _active;
        private Func<bool> _updateFunc; // Destroy Updater if return true;

        // Holds a reference to all active updaters
        private static List<UtilityHelper_FunctionUpdater> _updaterList;
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
                _initGameObject = new GameObject("FunctionUpdater_Global");
                _updaterList = new List<UtilityHelper_FunctionUpdater>();
            }
        }


        public static UtilityHelper_FunctionUpdater Create(Action updateFunc) =>
            Create(() => { updateFunc(); return false; }, "", true, false);

        public static UtilityHelper_FunctionUpdater Create(Action updateFunc, string functionName) =>
            Create(() => { updateFunc(); return false; }, functionName, true, false);

        public static UtilityHelper_FunctionUpdater Create(Func<bool> updateFunc) =>
            Create(updateFunc, "", true, false);

        public static UtilityHelper_FunctionUpdater Create(Func<bool> updateFunc, string functionName) =>
            Create(updateFunc, functionName, true, false);

        public static UtilityHelper_FunctionUpdater Create(Func<bool> updateFunc, string functionName, bool active) =>
            Create(updateFunc, functionName, active, false);

        public static UtilityHelper_FunctionUpdater Create(Func<bool> updateFunc, string functionName,
            bool active, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
                StopAllUpdatersWithName(functionName);

            GameObject gameObject = new GameObject("FunctionUpdater Object " + functionName, typeof(MonoBehaviourHook));
            UtilityHelper_FunctionUpdater functionUpdater = new UtilityHelper_FunctionUpdater(gameObject, updateFunc, functionName, active);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionUpdater.Update;

            _updaterList.Add(functionUpdater);
            return functionUpdater;
        }

        private static void RemoveUpdater(UtilityHelper_FunctionUpdater funcUpdater)
        {
            InitIfNeeded();
            _updaterList.Remove(funcUpdater);
        }

        public static void DestroyUpdater(UtilityHelper_FunctionUpdater funcUpdater)
        {
            InitIfNeeded();

            if (funcUpdater != null)
                funcUpdater.DestroySelf();
        }

        public static void StopUpdaterWithName(string functionName)
        {
            InitIfNeeded();

            for (int i = 0; i < _updaterList.Count; i++)
            {
                if (_updaterList[i]._functionName == functionName)
                {
                    _updaterList[i].DestroySelf();
                    return;
                }
            }
        }

        public static void StopAllUpdatersWithName(string functionName)
        {
            InitIfNeeded();

            for (int i = 0; i < _updaterList.Count; i++)
            {
                if (_updaterList[i]._functionName == functionName)
                {
                    _updaterList[i].DestroySelf();
                    i--;
                }
            }
        }

        
        public UtilityHelper_FunctionUpdater(GameObject gameObject, Func<bool> updateFunc, string functionName, bool active)
        {
            this._gameObject = gameObject;
            this._updateFunc = updateFunc;
            this._functionName = functionName;
            this._active = active;
        }

        public void Pause() => _active = false;

        public void Resume() => _active = true;

        private void Update()
        {
            if (!_active)
                return;

            if (_updateFunc())
                DestroySelf();
        }

        public void DestroySelf()
        {
            RemoveUpdater(this);

            if (_gameObject != null)
                UnityEngine.Object.Destroy(_gameObject);
        }
    }
}