using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Electrode : MonoBehaviour
{
    [SerializeField] private ParticleSystem _weldEffect;

    [SerializeField] private bool _inHolder;

    public UnityEvent OnWeld;

    public void SetInHolder(bool inHolder)
    {
        _inHolder = inHolder;

        WeldProcess.Instance.SetCriterion(CriterionName.ElectrodeInHolder, true);
    }

    public void Weld(bool welding, MeshRenderer seam)
    {
        if (_inHolder)
        {
            if (welding)
            {          
                seam.enabled = true;
                OnWeld?.Invoke();
            }
        }
    }

    public void ActivateFlash(bool activate)
    {
        _weldEffect.gameObject.SetActive(activate);
    }
}
