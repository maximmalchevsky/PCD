using UnityEngine;

namespace Task6_7
{
    public class TrainController : MonoBehaviour
    {
        public enum TrainState
        {
            Approaching,
            Decelerating,
            StoppedAtStation,
            Departing
        }

        public float maxSpeed = 16f;
        public float acceleration = 3f;
        public float deceleration = 4f;
        public float stationWaitDuration = 5f;
        
        public float startZ = -70f;
        public float stationZ = 0f;
        public float endZ = 70f;
        public float slowDownDistance = 25f;

        private float currentSpeed = 0f;
        private float waitTimer = 0f;
        private TrainState state = TrainState.Approaching;

        void Start()
        {
            Vector3 pos = transform.position;
            pos.z = startZ;
            transform.position = pos;
            state = TrainState.Approaching;
            currentSpeed = maxSpeed * 0.6f;
        }

        void Update()
        {
            switch (state)
            {
                case TrainState.Approaching:
                    currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
                    if (transform.position.z >= stationZ - slowDownDistance)
                    {
                        state = TrainState.Decelerating;
                    }
                    break;

                case TrainState.Decelerating:
                    float distToStation = stationZ - transform.position.z;
                    float desiredSpeed = Mathf.Lerp(0.5f, maxSpeed, distToStation / slowDownDistance);
                    currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, deceleration * Time.deltaTime);
                    if (transform.position.z >= stationZ - 0.2f)
                    {
                        Vector3 p = transform.position;
                        p.z = stationZ;
                        transform.position = p;
                        currentSpeed = 0f;
                        waitTimer = 0f;
                        state = TrainState.StoppedAtStation;
                    }
                    break;

                case TrainState.StoppedAtStation:
                    currentSpeed = 0f;
                    waitTimer += Time.deltaTime;
                    if (waitTimer >= stationWaitDuration)
                    {
                        state = TrainState.Departing;
                    }
                    break;

                case TrainState.Departing:
                    currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
                    if (transform.position.z >= endZ)
                    {
                        Vector3 resetPos = transform.position;
                        resetPos.z = startZ;
                        transform.position = resetPos;
                        currentSpeed = maxSpeed * 0.5f;
                        state = TrainState.Approaching;
                    }
                    break;
            }

            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
        }

        public TrainState GetState()
        {
            return state;
        }

        public string GetStateDescription()
        {
            switch (state)
            {
                case TrainState.Approaching:
                    return "Прибытие к станции";
                case TrainState.Decelerating:
                    return "Торможение перед платформой";
                case TrainState.StoppedAtStation:
                    return "Стоянка на платформе (высадка/посадка)";
                case TrainState.Departing:
                    return "Отправление со станции";
                default:
                    return "В пути";
            }
        }

        public float GetSpeed()
        {
            return currentSpeed;
        }

        public float GetWaitTimeRemaining()
        {
            if (state == TrainState.StoppedAtStation)
            {
                return Mathf.Max(0f, stationWaitDuration - waitTimer);
            }
            return 0f;
        }
    }
}
