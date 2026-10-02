using UnityEngine;

namespace Task6_7
{
    public class PedestrianAgent : MonoBehaviour
    {
        public Transform[] waypoints;
        public float speed = 2f;
        public float rotationSpeed = 6f;
        public float stoppingDistance = 0.6f;

        public int currentWaypointIndex = 0;
        private Animator animator;
        private float bobTimer = 0f;
        private Transform visualModel;

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.applyRootMotion = false;
            }
            if (transform.childCount > 0)
            {
                visualModel = transform.GetChild(0);
            }
        }

        public void SetInitialWaypointIndex(int index)
        {
            if (waypoints != null && waypoints.Length > 0)
            {
                currentWaypointIndex = (index + 1) % waypoints.Length;
            }
        }

        void Update()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            Transform target = waypoints[currentWaypointIndex];
            Vector3 flatTarget = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 direction = (flatTarget - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            Vector3 nextPos = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            bobTimer += Time.deltaTime * speed * 5f;
            if (animator == null)
            {
                float bobOffset = Mathf.Abs(Mathf.Sin(bobTimer)) * 0.05f;
                nextPos.y = target.position.y + bobOffset;
                if (visualModel != null)
                {
                    float tiltAngle = Mathf.Sin(bobTimer) * 4f;
                    visualModel.localRotation = Quaternion.Euler(0f, 0f, tiltAngle);
                }
            }

            Vector3 moveDelta = nextPos - transform.position;
            if (moveDelta.sqrMagnitude > 0.0001f)
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position + Vector3.up * 0.4f, 0.25f, moveDelta.normalized, out hit, moveDelta.magnitude + 0.15f))
                {
                    if (hit.collider != null && !hit.collider.transform.IsChildOf(transform) && !hit.collider.isTrigger)
                    {
                        Vector3 slide = Vector3.ProjectOnPlane(moveDelta, hit.normal);
                        if (slide.sqrMagnitude > 0.0001f)
                        {
                            nextPos = transform.position + slide.normalized * Mathf.Min(moveDelta.magnitude, slide.magnitude);
                        }
                        else
                        {
                            nextPos = transform.position;
                        }
                    }
                }
            }

            transform.position = nextPos;

            if (Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(target.position.x, 0f, target.position.z)) < stoppingDistance)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
        }
    }
}
