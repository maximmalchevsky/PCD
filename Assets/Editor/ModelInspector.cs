using UnityEngine;
using UnityEditor;

namespace CityEditor
{
    public static class ModelInspector
    {
        [MenuItem("City/Inspect Models")]
        public static void Inspect()
        {
            string[] models = new string[]
            {
                "Assets/Resources/tractor-shovel.fbx",
                "Assets/Resources/truck-flat.fbx",
                "Assets/Resources/wheel-tractor-back.fbx",
                "Assets/Resources/wheel-tractor-front.fbx",
                "Assets/Resources/wheel-truck.fbx",
                "Assets/Resources/train-carriage-dirt.fbx",
                "Assets/Resources/tractor.fbx",
                "Assets/Resources/container-blue.obj",
                "Assets/Resources/container-red.obj"
            };

            foreach (string path in models)
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (go == null)
                {
                    Debug.LogWarning("NOT FOUND: " + path);
                    continue;
                }

                Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
                Bounds totalBounds = new Bounds();
                bool first = true;
                string childNames = "";
                foreach (Renderer r in renderers)
                {
                    if (first) { totalBounds = r.bounds; first = false; }
                    else { totalBounds.Encapsulate(r.bounds); }
                    childNames += r.gameObject.name + " (" + r.bounds.size + "); ";
                }

                Debug.Log("MODEL: " + go.name + " | TotalBounds Center: " + totalBounds.center + " Size: " + totalBounds.size + " | Children: " + childNames);
            }
        }
    }
}
