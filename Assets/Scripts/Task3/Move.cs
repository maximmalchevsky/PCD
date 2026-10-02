using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
        Transform t = GetComponent<Transform>();
        var kb = Keyboard.current;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || (kb != null && (kb.rightArrowKey.isPressed || kb.dKey.isPressed)))
            t.position += new Vector3(0.1f, 0f, 0f);
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || (kb != null && (kb.leftArrowKey.isPressed || kb.aKey.isPressed)))
            t.position += new Vector3(-0.1f, 0f, 0f);

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || (kb != null && (kb.upArrowKey.isPressed || kb.wKey.isPressed)))
            t.position += new Vector3(0f, 0f, 0.1f);
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || (kb != null && (kb.downArrowKey.isPressed || kb.sKey.isPressed)))
            t.position += new Vector3(0f, 0f, -0.1f);

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.PageUp) || (kb != null && (kb.spaceKey.isPressed || kb.eKey.isPressed || kb.pageUpKey.isPressed)))
            t.position += new Vector3(0f, 0.1f, 0f);
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.PageDown) || (kb != null && (kb.leftShiftKey.isPressed || kb.qKey.isPressed || kb.pageDownKey.isPressed)))
            t.position += new Vector3(0f, -0.1f, 0f);
    }
}
