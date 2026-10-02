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
            SnapToGround();
        }

        private void SnapToGround()
        {
            RaycastHit[] groundHits = Physics.RaycastAll(transform.position + Vector3.up * 2f, Vector3.down, 5f);
            float maxGroundY = float.MinValue;
            bool foundGround = false;
            for (int gi = 0; gi < groundHits.Length; gi++)
            {
                if (!groundHits[gi].collider.transform.IsChildOf(transform) && !groundHits[gi].collider.isTrigger)
                {
                    if (groundHits[gi].point.y > maxGroundY)
                    {
                        maxGroundY = groundHits[gi].point.y;
                        foundGround = true;
                    }
                }
            }
            if (foundGround)
            {
                Vector3 p = transform.position;
                p.y = maxGroundY;
                transform.position = p;
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
            if (animator == null && visualModel != null)
            {
                float tiltAngle = Mathf.Sin(bobTimer) * 4f;
                visualModel.localRotation = Quaternion.Euler(0f, 0f, tiltAngle);
            }

            RaycastHit[] groundHits = Physics.RaycastAll(nextPos + Vector3.up * 1.5f, Vector3.down, 4.0f);
            float maxGroundY = float.MinValue;
            bool foundGround = false;
            for (int gi = 0; gi < groundHits.Length; gi++)
            {
                if (!groundHits[gi].collider.transform.IsChildOf(transform) && !groundHits[gi].collider.isTrigger)
                {
                    if (groundHits[gi].point.y > maxGroundY)
                    {
                        maxGroundY = groundHits[gi].point.y;
                        foundGround = true;
                    }
                }
            }
            if (foundGround)
            {
                float bobOffset = (animator == null) ? Mathf.Abs(Mathf.Sin(bobTimer)) * 0.05f : 0f;
                float targetY = maxGroundY + bobOffset;
                nextPos.y = Mathf.MoveTowards(transform.position.y, targetY, 6f * Time.deltaTime);
            }

            Vector3 horizDelta = new Vector3(nextPos.x - transform.position.x, 0f, nextPos.z - transform.position.z);
            if (horizDelta.sqrMagnitude > 0.0001f)
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position + Vector3.up * 0.55f, 0.22f, horizDelta.normalized, out hit, horizDelta.magnitude + 0.15f))
                {
                    if (hit.collider != null && !hit.collider.transform.IsChildOf(transform) && !hit.collider.isTrigger)
                    {
                        Vector3 slide = Vector3.ProjectOnPlane(horizDelta, hit.normal);
                        if (slide.sqrMagnitude > 0.0001f)
                        {
                            Vector3 adjusted = slide.normalized * Mathf.Min(horizDelta.magnitude, slide.magnitude);
                            nextPos.x = transform.position.x + adjusted.x;
                            nextPos.z = transform.position.z + adjusted.z;
                        }
                        else
                        {
                            nextPos.x = transform.position.x;
                            nextPos.z = transform.position.z;
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
