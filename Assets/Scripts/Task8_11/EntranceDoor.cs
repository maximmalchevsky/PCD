using UnityEngine;

namespace Task8_11
{
    public class EntranceDoor : MonoBehaviour
    {
        public Transform doorLeaf;
        public float openAngle = 85f;
        public float speed = 4f;
        public bool isOpen = false;

        private Quaternion closedRot;
        private Quaternion openRot;

        void Start()
        {
            if (doorLeaf != null)
            {
                closedRot = doorLeaf.localRotation;
                openRot = closedRot * Quaternion.Euler(0f, -openAngle, 0f);
            }
        }

        void Update()
        {
            if (doorLeaf != null)
            {
                Quaternion target = isOpen ? openRot : closedRot;
                doorLeaf.localRotation = Quaternion.Slerp(doorLeaf.localRotation, target, Time.deltaTime * speed);
            }
        }

        public void Toggle()
        {
            isOpen = !isOpen;
        }
    }
}
