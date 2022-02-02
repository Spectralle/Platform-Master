using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    // Calls function on every Update until it returns true
    public class Utilities_FunctionUpdater
    {
        private GameObject gameObject;
        private string functionName;
        private bool active;
        private Func<bool> updateFunc; // Destroy Updater if return true;

        // Holds a reference to all active updaters
        private static List<Utilities_FunctionUpdater> updaterList;
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
                initGameObject = new GameObject("FunctionUpdater_Global");
                updaterList = new List<Utilities_FunctionUpdater>();
            }
        }


        public static Utilities_FunctionUpdater Create(Action updateFunc) =>
            Create(() => { updateFunc(); return false; }, "", true, false);

        public static Utilities_FunctionUpdater Create(Action updateFunc, string functionName) =>
            Create(() => { updateFunc(); return false; }, functionName, true, false);

        public static Utilities_FunctionUpdater Create(Func<bool> updateFunc) =>
            Create(updateFunc, "", true, false);

        public static Utilities_FunctionUpdater Create(Func<bool> updateFunc, string functionName) =>
            Create(updateFunc, functionName, true, false);

        public static Utilities_FunctionUpdater Create(Func<bool> updateFunc, string functionName, bool active) =>
            Create(updateFunc, functionName, active, false);

        public static Utilities_FunctionUpdater Create(Func<bool> updateFunc, string functionName,
            bool active, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
                StopAllUpdatersWithName(functionName);

            GameObject gameObject = new GameObject("FunctionUpdater Object " + functionName, typeof(MonoBehaviourHook));
            Utilities_FunctionUpdater functionUpdater = new Utilities_FunctionUpdater(gameObject, updateFunc, functionName, active);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionUpdater.Update;

            updaterList.Add(functionUpdater);
            return functionUpdater;
        }

        private static void RemoveUpdater(Utilities_FunctionUpdater funcUpdater)
        {
            InitIfNeeded();
            updaterList.Remove(funcUpdater);
        }

        public static void DestroyUpdater(Utilities_FunctionUpdater funcUpdater)
        {
            InitIfNeeded();

            if (funcUpdater != null)
                funcUpdater.DestroySelf();
        }

        public static void StopUpdaterWithName(string functionName)
        {
            InitIfNeeded();

            for (int i = 0; i < updaterList.Count; i++)
            {
                if (updaterList[i].functionName == functionName)
                {
                    updaterList[i].DestroySelf();
                    return;
                }
            }
        }

        public static void StopAllUpdatersWithName(string functionName)
        {
            InitIfNeeded();

            for (int i = 0; i < updaterList.Count; i++)
            {
                if (updaterList[i].functionName == functionName)
                {
                    updaterList[i].DestroySelf();
                    i--;
                }
            }
        }

        
        public Utilities_FunctionUpdater(GameObject gameObject, Func<bool> updateFunc, string functionName, bool active)
        {
            this.gameObject = gameObject;
            this.updateFunc = updateFunc;
            this.functionName = functionName;
            this.active = active;
        }

        public void Pause() => active = false;

        public void Resume() => active = true;

        private void Update()
        {
            if (!active)
                return;

            if (updateFunc())
                DestroySelf();
        }

        public void DestroySelf()
        {
            RemoveUpdater(this);

            if (gameObject != null)
                UnityEngine.Object.Destroy(gameObject);
        }
    }
}