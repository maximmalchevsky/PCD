using UnityEngine;

namespace Task8_11
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickupableItem : MonoBehaviour
    {
        public string itemName = "Предмет";
        public CabinetDoor parentCabinetDoor;

        [HideInInspector]
        public Rigidbody rb;

        void Awake()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
        }

        public bool CanPickUp(out string reason)
        {
            if (parentCabinetDoor != null && !parentCabinetDoor.isOpen)
            {
                float distToDoor = Vector3.Distance(transform.position, parentCabinetDoor.transform.position);
                if (distToDoor < 0.85f)
                {
                    reason = "Шкаф закрыт! Сначала откройте дверь.";
                    return false;
                }
                else
                {
                    parentCabinetDoor = null;
                }
            }
            reason = string.Empty;
            return true;
        }
    }
}
