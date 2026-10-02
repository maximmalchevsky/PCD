using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Scale : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
        Transform t = GetComponent<Transform>();
        var kb = Keyboard.current;

        if (Input.GetKey(KeyCode.U) || (kb != null && (kb.uKey.isPressed || kb.numpad7Key.isPressed)))
            t.localScale += new Vector3(0.05f, 0f, 0f);
        if (Input.GetKey(KeyCode.J) || (kb != null && (kb.jKey.isPressed || kb.numpad4Key.isPressed)))
            t.localScale += new Vector3(-0.05f, 0f, 0f);

        if (Input.GetKey(KeyCode.I) || (kb != null && (kb.iKey.isPressed || kb.numpad8Key.isPressed)))
            t.localScale += new Vector3(0f, 0.05f, 0f);
        if (Input.GetKey(KeyCode.K) || (kb != null && (kb.kKey.isPressed || kb.numpad5Key.isPressed)))
            t.localScale += new Vector3(0f, -0.05f, 0f);

        if (Input.GetKey(KeyCode.O) || (kb != null && (kb.oKey.isPressed || kb.numpad9Key.isPressed)))
            t.localScale += new Vector3(0f, 0f, 0.05f);
        if (Input.GetKey(KeyCode.L) || (kb != null && (kb.lKey.isPressed || kb.numpad6Key.isPressed)))
            t.localScale += new Vector3(0f, 0f, -0.05f);
    }
}
