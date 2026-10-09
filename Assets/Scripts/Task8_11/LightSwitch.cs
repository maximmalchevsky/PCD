using UnityEngine;

namespace Task8_11
{
    public class LightSwitch : MonoBehaviour
    {
        public Light[] targetLights;
        public Transform toggleLever;
        public bool isOn = true;
        public float leverAngle = 25f;

        private Quaternion onRot;
        private Quaternion offRot;

        void Start()
        {
            if (toggleLever != null)
            {
                onRot = Quaternion.Euler(-leverAngle, 0f, 0f);
                offRot = Quaternion.Euler(leverAngle, 0f, 0f);
                toggleLever.localRotation = isOn ? onRot : offRot;
            }
            ApplyLighting();
        }

        public void Toggle()
        {
            isOn = !isOn;
            if (toggleLever != null)
            {
                toggleLever.localRotation = isOn ? onRot : offRot;
            }
            ApplyLighting();
        }

        private void ApplyLighting()
        {
            if (targetLights == null) return;
            for (int i = 0; i < targetLights.Length; i++)
            {
                if (targetLights[i] != null)
                {
                    targetLights[i].enabled = isOn;
                }
            }
        }
    }
}
