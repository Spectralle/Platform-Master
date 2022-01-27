using UnityEngine;

namespace Utilities
{
    public static partial class UtilityHelper
    {
        public static Transform CloneTransform(Transform transform, string name = null)
        {
            Transform clone = GameObject.Instantiate(transform, transform.parent);

            if (name != null)
                clone.name = name;
            else
                clone.name = transform.name;

            return clone;
        }

        public static Transform CloneTransform(Transform transform, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
        {
            Transform clone = CloneTransform(transform, name);
            clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
            if (setActiveTrue)
                clone.gameObject.SetActive(true);
            return clone;
        }

        public static Transform CloneTransform(Transform transform, Transform newParent, string name = null)
        {
            Transform clone = GameObject.Instantiate(transform, newParent);

            if (name != null)
                clone.name = name;
            else
                clone.name = transform.name;

            return clone;
        }

        public static Transform CloneTransform(Transform transform, Transform newParent, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
        {
            Transform clone = CloneTransform(transform, newParent, name);
            clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
            if (setActiveTrue)
                clone.gameObject.SetActive(true);
            return clone;
        }

        public static Transform CloneTransformWorld(Transform transform, Transform newParent, string name, Vector3 newLocalPosition)
        {
            Transform clone = CloneTransform(transform, newParent, name);
            clone.localPosition = newLocalPosition;
            return clone;
        }


        // Destroy all children of this parent
        public static void DestroyChildren(Transform parent)
        {
            foreach (Transform transform in parent)
                GameObject.Destroy(transform.gameObject);
        }

        // Destroy all children and randomize their names, useful if you want to do a Find() after calling destroy, since they only really get destroyed at the end of the frame
        public static void DestroyChildrenRandomizeNames(Transform parent)
        {
            foreach (Transform transform in parent)
            {
                transform.name = "" + UnityEngine.Random.Range(10000, 99999);
                GameObject.Destroy(transform.gameObject);
            }
        }

        // Destroy all children except the ones with these names
        public static void DestroyChildren(Transform parent, params string[] ignoreArr)
        {
            foreach (Transform transform in parent)
            {
                if (System.Array.IndexOf(ignoreArr, transform.name) == -1) // Don't ignore
                    GameObject.Destroy(transform.gameObject);
            }
        }


        // Set all parent and all children to this layer
        public static void SetAllChildrenLayer(Transform parent, int layer)
        {
            parent.gameObject.layer = layer;
            foreach (Transform trans in parent)
                SetAllChildrenLayer(trans, layer);
        }
    }
}
