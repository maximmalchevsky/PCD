using UnityEngine;
using UnityEditor;

public class CheckTruckFlat {
    [MenuItem("Tools/Check Truck Flat")]
    public static void Check() {
        GameObject go = Resources.Load<GameObject>("truck-flat");
        if (go != null) {
            foreach (Transform t in go.GetComponentsInChildren<Transform>()) {
                Debug.Log($"Truck child: {t.name}, pos={t.localPosition}, rot={t.localEulerAngles}, scale={t.localScale}");
            }
        }
    }
}
