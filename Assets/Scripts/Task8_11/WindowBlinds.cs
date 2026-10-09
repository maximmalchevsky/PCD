using UnityEngine;

namespace Task8_11
{
    public class WindowBlinds : MonoBehaviour
    {
        public Transform[] slats;
        public Light windowLight;
        public float closedAngle = 84f;
        public float speed = 5f;
        public bool isOpen = true;

        private Quaternion openRot = Quaternion.Euler(0f, 0f, 0f);
        private Quaternion closedRot;
        private float baseLightIntensity = 1.8f;

        void Start()
        {
            closedRot = Quaternion.Euler(closedAngle, 0f, 0f);
            if (windowLight != null)
            {
                baseLightIntensity = windowLight.intensity;
            }
        }

        void Update()
        {
            if (slats != null)
            {
                Quaternion target = isOpen ? openRot : closedRot;
                for (int i = 0; i < slats.Length; i++)
                {
                    if (slats[i] != null)
                    {
                        slats[i].localRotation = Quaternion.Slerp(slats[i].localRotation, target, Time.deltaTime * speed);
                    }
                }
            }

            if (windowLight != null)
            {
                float targetIntensity = isOpen ? baseLightIntensity : 0.15f;
                windowLight.intensity = Mathf.MoveTowards(windowLight.intensity, targetIntensity, Time.deltaTime * speed * 0.8f);
            }
        }

        public void Toggle()
        {
            isOpen = !isOpen;
        }
    }
}
