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
            rb = GetComponent<Rigidbody>();
        }

        public bool CanPickUp(out string reason)
        {
            if (parentCabinetDoor != null && !parentCabinetDoor.isOpen)
            {
                reason = "Шкаф закрыт! Сначала откройте дверь.";
                return false;
            }
            reason = string.Empty;
            return true;
        }
    }
}
