using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class Holder : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor _socket;

    [SerializeField] private bool _electrodeInHolder;

    [SerializeField] private Electrode _electrode;

    [SerializeField] private bool _voltage;

    public void SetElectrodeInHolder()
    {
        if(_socket.firstInteractableSelected.transform.TryGetComponent(out Electrode electrode))
        {
            electrode.SetInHolder(true);
            WeldProcess.Instance.SetCriterion(CriterionName.ElectrodeInHolder, true);
            electrode.GetComponent<Rigidbody>().isKinematic = false;
            electrode.GetComponent<Rigidbody>().useGravity = true;
            _electrodeInHolder = true;
        }
    }

    public void SetVoltage(bool voltage)
    {
        _voltage = voltage;

        if (_electrodeInHolder)
        {
            _electrode.SetInHolder(_voltage);
        }
    }

    public void RemoveElectrodeFromHolder()
    {
        if (_socket.firstInteractableSelected.transform.TryGetComponent(out Electrode electrode))
        {
            electrode.SetInHolder(false);
            _electrodeInHolder = false;
            _electrode.SetInHolder(false);
        }
    }
}
