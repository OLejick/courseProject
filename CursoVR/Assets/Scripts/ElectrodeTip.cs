using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElectrodeTip : MonoBehaviour
{
    [SerializeField] private Electrode _electrode;

    private void OnTriggerEnter(Collider col)
    {    
        if (col.TryGetComponent(out Seam seam))
        {
            _electrode.Weld(true, col.GetComponent<MeshRenderer>());
        }
        if(col.TryGetComponent(out FlashActivator FlashActivator))
        {
            _electrode.ActivateFlash(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.TryGetComponent(out Seam seam))
        {
            _electrode.Weld(false, col.GetComponent<MeshRenderer>());
        }
        if (col.TryGetComponent(out FlashActivator FlashActivator))
        {
            _electrode.ActivateFlash(false);
        }
    }
}
