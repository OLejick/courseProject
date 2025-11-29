using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Gloves : MonoBehaviour
{
    [SerializeField] private Transform _leftGlovePrefab;
    [SerializeField] private Transform _rightGlovePrefab;

    [SerializeField] private GameObject _currentLeftHand;
    [SerializeField] private GameObject _currentRightHand;

    [SerializeField] private ActionBasedController _leftController;
    [SerializeField] private ActionBasedController _rightController;

    [SerializeField] private UnityEvent OnHandsChanged;

    private void Start()
    {
        SetCurrentHands();
    }

    public void PutOn()
    {
        Destroy(_currentLeftHand);
        Destroy(_currentRightHand);

        _leftController.model = Instantiate(_leftGlovePrefab, _leftController.modelParent).transform;
        _rightController.model = Instantiate(_rightGlovePrefab, _rightController.modelParent).transform;

        SetCurrentHands();

        OnHandsChanged?.Invoke();
    }

    public void SetCurrentHands()
    {
        _currentLeftHand = _leftController.model.gameObject;
        _currentRightHand = _rightController.model.gameObject;

        if(_currentLeftHand.TryGetComponent(out WeldGloves weldGloves))
        {
            WeldProcess.Instance.SetCriterion(CriterionName.Gloves, true);
        }
        else
        {
            WeldProcess.Instance.SetCriterion(CriterionName.Gloves, false);
        }
    }
}
