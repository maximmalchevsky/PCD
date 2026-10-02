using UnityEngine;

namespace Task6_7
{
    public class TrafficLightController : MonoBehaviour
    {
        public enum LightState
        {
            Green,
            Yellow,
            Red
        }

        public LightState state = LightState.Green;
        public float greenDuration = 8f;
        public float yellowDuration = 2.5f;
        public float redDuration = 10.5f;

        public Renderer redRenderer;
        public Renderer yellowRenderer;
        public Renderer greenRenderer;

        public Material redOnMat;
        public Material redOffMat;
        public Material yellowOnMat;
        public Material yellowOffMat;
        public Material greenOnMat;
        public Material greenOffMat;

        public Collider stopCollider;

        private float timer = 0f;

        void Start()
        {
            UpdateVisuals();
        }

        void Update()
        {
            timer += Time.deltaTime;
            switch (state)
            {
                case LightState.Green:
                    if (timer >= greenDuration)
                    {
                        state = LightState.Yellow;
                        timer = 0f;
                        UpdateVisuals();
                    }
                    break;
                case LightState.Yellow:
                    if (timer >= yellowDuration)
                    {
                        state = LightState.Red;
                        timer = 0f;
                        UpdateVisuals();
                    }
                    break;
                case LightState.Red:
                    if (timer >= redDuration)
                    {
                        state = LightState.Green;
                        timer = 0f;
                        UpdateVisuals();
                    }
                    break;
            }
        }

        public bool IsRedOrYellow()
        {
            return state == LightState.Red || state == LightState.Yellow;
        }

        public void SetInitialState(LightState initialState, float startOffset)
        {
            state = initialState;
            timer = startOffset;
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            if (redRenderer != null && redOnMat != null && redOffMat != null)
            {
                redRenderer.sharedMaterial = (state == LightState.Red) ? redOnMat : redOffMat;
            }
            if (yellowRenderer != null && yellowOnMat != null && yellowOffMat != null)
            {
                yellowRenderer.sharedMaterial = (state == LightState.Yellow) ? yellowOnMat : yellowOffMat;
            }
            if (greenRenderer != null && greenOnMat != null && greenOffMat != null)
            {
                greenRenderer.sharedMaterial = (state == LightState.Green) ? greenOnMat : greenOffMat;
            }

            if (stopCollider != null)
            {
                stopCollider.enabled = IsRedOrYellow();
            }
        }
    }
}
