using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class WeldDevice : MonoBehaviour
{
    [SerializeField] private Transform _switch;
    [SerializeField] private Material _enabledSwitch;
    [SerializeField] private Material _disabledSwitch;

    [SerializeField] private bool _isOn;
    public bool IsOn => _isOn;

    [SerializeField] private Holder _holder;
    [SerializeField] private DetailWire _detailWire;

    public void Switching()
    {
        _isOn = !_isOn;

        if (_isOn)
        {
            _switch.transform.rotation = Quaternion.Euler(_switch.transform.rotation.x, _switch.transform.rotation.y, 80);
            _switch.GetComponent<MeshRenderer>().material = _enabledSwitch;

            WeldProcess.Instance.SetCriterion(CriterionName.SwitchOn, true);
            _holder.SetVoltage(true);
            _detailWire.SetVoltage(true);
        }
        else
        {
            _switch.transform.rotation = Quaternion.Euler(_switch.transform.rotation.x, _switch.transform.rotation.y, 100);
            _switch.GetComponent<MeshRenderer>().material = _disabledSwitch;

            _holder.SetVoltage(false);
            _detailWire.SetVoltage(false);

            if (WeldProcess.Instance.DetailIsComplete)
                WeldProcess.Instance.SetCriterion(CriterionName.SwitchOff, true);
        }

    }
}
