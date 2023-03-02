using UnityEngine;

namespace Assets.Scripts.HelpersUnity
{
    public static class EditorHelper
    {
        public static Transform FindRecursive(Transform root, string gameObjectName)
        {
            var childTransforms = root.GetComponentsInChildren<Transform>();
            foreach (var child in childTransforms)
            {
                if (child.gameObject.name == gameObjectName)
                {
                    return child;
                }
            }

            return null;
        }
    }
}