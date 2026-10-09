using UnityEngine;

namespace Task12_16
{
    public class WarningBeacon : MonoBehaviour
    {
        public Light beaconLight;
        public Renderer lensRenderer;
        public float flashFrequency = 1.6f;
        public Color beaconColor = new Color(1f, 0.55f, 0.05f);

        private Material lensMat;

        void Start()
        {
            if (beaconLight == null)
            {
                beaconLight = GetComponentInChildren<Light>();
            }

            if (beaconLight != null)
            {
                beaconLight.color = beaconColor;
            }

            if (lensRenderer != null)
            {
                lensMat = lensRenderer.material;
            }
        }

        void Update()
        {
            float wave = (Mathf.Sin(Time.time * flashFrequency * Mathf.PI * 2f) + 1f) * 0.5f;
            float pulse = wave * wave;

            if (beaconLight != null)
            {
                beaconLight.intensity = Mathf.Lerp(0.1f, 3.2f, pulse);
            }

            if (lensMat != null)
            {
                Color em = beaconColor * Mathf.Lerp(0.2f, 2.5f, pulse);
                if (lensMat.HasProperty("_EmissionColor"))
                {
                    lensMat.SetColor("_EmissionColor", em);
                }
            }
        }
    }
}
