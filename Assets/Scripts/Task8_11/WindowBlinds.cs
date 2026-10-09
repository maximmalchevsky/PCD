using UnityEngine;

namespace Task8_11
{
    public class WindowBlinds : MonoBehaviour
    {
        public Transform[] slats;
        public float closedAngle = 78f;
        public float speed = 4f;
        public bool isOpen = true;

        private Quaternion openRot = Quaternion.Euler(0f, 0f, 0f);
        private Quaternion closedRot;

        void Start()
        {
            closedRot = Quaternion.Euler(closedAngle, 0f, 0f);
        }

        void Update()
        {
            if (slats == null) return;
            Quaternion target = isOpen ? openRot : closedRot;
            for (int i = 0; i < slats.Length; i++)
            {
                if (slats[i] != null)
                {
                    slats[i].localRotation = Quaternion.Slerp(slats[i].localRotation, target, Time.deltaTime * speed);
                }
            }
        }

        public void Toggle()
        {
            isOpen = !isOpen;
        }
    }
}
