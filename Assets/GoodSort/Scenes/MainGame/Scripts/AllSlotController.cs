using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllSlotController : MonoBehaviour
{
    [SerializeField] SlotController[] _slots;

    public SlotController[] GetSlots()
    {
        if(_slots==null|| _slots.Length == 0)
        {
            Debug.LogWarning("no slot in all slot controller, auto get child, that will make arrange child not allow design");

            List<SlotController> slot=  new List<SlotController>();
            
            foreach(Transform child in transform)
            {
                if (child.gameObject.GetComponent<SlotController>())
                {
                    slot.Add(child.gameObject.GetComponent<SlotController>());
                }
            }

            _slots= slot.ToArray();
        }

        return _slots;
    }
}
