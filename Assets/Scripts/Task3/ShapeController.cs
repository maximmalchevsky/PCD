using UnityEngine;
using UnityEngine.InputSystem;

public class ShapeController : MonoBehaviour
{
    public GameObject cube;
    public GameObject sphere;
    public GameObject capsule;
    public GameObject cylinder;

    GameObject target;
    string targetName = "Куб (1)";

    void Start()
    {
        if (cube == null) cube = GameObject.Find("Cube");
        if (sphere == null) sphere = GameObject.Find("Sphere");
        if (capsule == null) capsule = GameObject.Find("Capsule");
        if (cylinder == null) cylinder = GameObject.Find("Cylinder");

        target = cube;

        if (capsule != null)
        {
            capsule.transform.rotation = Quaternion.identity;
        }
    }

    void Update()
    {
        var kb = Keyboard.current;

        if (Input.GetKeyDown(KeyCode.Alpha1) || (kb != null && kb.digit1Key.wasPressedThisFrame)) { target = cube; targetName = "Куб (1)"; }
        if (Input.GetKeyDown(KeyCode.Alpha2) || (kb != null && kb.digit2Key.wasPressedThisFrame)) { target = sphere; targetName = "Сфера (2)"; }
        if (Input.GetKeyDown(KeyCode.Alpha3) || (kb != null && kb.digit3Key.wasPressedThisFrame)) { target = capsule; targetName = "Капсула (3)"; }
        if (Input.GetKeyDown(KeyCode.Alpha4) || (kb != null && kb.digit4Key.wasPressedThisFrame)) { target = cylinder; targetName = "Цилиндр (4)"; }

        if (target == null) return;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || (kb != null && kb.dKey.isPressed)) target.transform.position += new Vector3(0.1f, 0, 0);
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || (kb != null && kb.aKey.isPressed)) target.transform.position += new Vector3(-0.1f, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || (kb != null && kb.wKey.isPressed)) target.transform.position += new Vector3(0, 0, 0.1f);
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || (kb != null && kb.sKey.isPressed)) target.transform.position += new Vector3(0, 0, -0.1f);
        if (Input.GetKey(KeyCode.Space) || (kb != null && kb.spaceKey.isPressed)) target.transform.position += new Vector3(0, 0.1f, 0);
        if (Input.GetKey(KeyCode.LeftShift) || (kb != null && kb.leftShiftKey.isPressed)) target.transform.position += new Vector3(0, -0.1f, 0);

        if (Input.GetKey(KeyCode.U) || (kb != null && kb.uKey.isPressed)) target.transform.localScale += new Vector3(0.05f, 0, 0);
        if (Input.GetKey(KeyCode.J) || (kb != null && kb.jKey.isPressed)) target.transform.localScale += new Vector3(-0.05f, 0, 0);
        if (Input.GetKey(KeyCode.I) || (kb != null && kb.iKey.isPressed)) target.transform.localScale += new Vector3(0, 0.05f, 0);
        if (Input.GetKey(KeyCode.K) || (kb != null && kb.kKey.isPressed)) target.transform.localScale += new Vector3(0, -0.05f, 0);
        if (Input.GetKey(KeyCode.O) || (kb != null && kb.oKey.isPressed)) target.transform.localScale += new Vector3(0, 0, 0.05f);
        if (Input.GetKey(KeyCode.L) || (kb != null && kb.lKey.isPressed)) target.transform.localScale += new Vector3(0, 0, -0.05f);

        if (Input.GetKey(KeyCode.X) || (kb != null && kb.xKey.isPressed)) target.transform.Rotate(1, 0, 0, Space.Self);
        if (Input.GetKey(KeyCode.Y) || (kb != null && kb.yKey.isPressed)) target.transform.Rotate(0, 1, 0, Space.Self);
        if (Input.GetKey(KeyCode.Z) || (kb != null && kb.zKey.isPressed)) target.transform.Rotate(0, 0, 1, Space.Self);
    }

    void OnGUI()
    {
        GUI.Box(new Rect(15, 15, 200, 35), "Выбрано: " + targetName);
    }
}
