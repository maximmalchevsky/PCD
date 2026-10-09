using UnityEngine;

namespace Task8_11
{
    public class CabinetDoor : MonoBehaviour
    {
        public Transform leftDoor;
        public Transform rightDoor;
        public float openAngle = 95f;
        public float speed = 5f;
        public bool isOpen = false;

        private Quaternion leftClosedRot;
        private Quaternion leftOpenRot;
        private Quaternion rightClosedRot;
        private Quaternion rightOpenRot;

        void Start()
        {
            if (leftDoor != null)
            {
                leftClosedRot = leftDoor.localRotation;
                leftOpenRot = leftClosedRot * Quaternion.Euler(0f, -openAngle, 0f);
            }
            if (rightDoor != null)
            {
                rightClosedRot = rightDoor.localRotation;
                rightOpenRot = rightClosedRot * Quaternion.Euler(0f, openAngle, 0f);
            }
        }

        void Update()
        {
            if (leftDoor != null)
            {
                Quaternion target = isOpen ? leftOpenRot : leftClosedRot;
                leftDoor.localRotation = Quaternion.Slerp(leftDoor.localRotation, target, Time.deltaTime * speed);
            }
            if (rightDoor != null)
            {
                Quaternion target = isOpen ? rightOpenRot : rightClosedRot;
                rightDoor.localRotation = Quaternion.Slerp(rightDoor.localRotation, target, Time.deltaTime * speed);
            }
        }

        public void Toggle()
        {
            isOpen = !isOpen;
        }

        public bool IsLookingAtDoorLeaf(Collider col)
        {
            if (col == null) return false;
            if (leftDoor != null && (col.transform == leftDoor || col.transform.IsChildOf(leftDoor))) return true;
            if (rightDoor != null && (col.transform == rightDoor || col.transform.IsChildOf(rightDoor))) return true;
            return false;
        }
    }
}
