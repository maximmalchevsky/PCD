using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rotate : MonoBehaviour
{
    float x = 0, y = 0, z = 0;

    void Start()
    {
        Vector3 currentAngles = transform.rotation.eulerAngles;
        x = currentAngles.x;
        y = currentAngles.y;
        z = currentAngles.z;
    }

    void Update()
    {
        Transform t = GetComponent<Transform>();
        var kb = Keyboard.current;

        if (Input.GetKey(KeyCode.X) || (kb != null && kb.xKey.isPressed)) x = x + 1;
        if (Input.GetKey(KeyCode.Y) || (kb != null && kb.yKey.isPressed)) y = y + 1;
        if (Input.GetKey(KeyCode.Z) || (kb != null && kb.zKey.isPressed)) z = z + 1;

        t.rotation = Quaternion.Euler(x, y, z);
    }
}
