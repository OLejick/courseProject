using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteTable : MonoBehaviour
{
    [SerializeField] private bool _flammableHere;
    [SerializeField] private GameObject _resultsUI;

    private void OnTriggerExit(Collider col)
    {
        if(col.TryGetComponent(out Flammable flammable))
        {
            _flammableHere = false;
            WeldProcess.Instance.SetCriterion(CriterionName.TableIsClear, true);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent(out Flammable flammable))
        {
            _flammableHere = true;
            WeldProcess.Instance.SetCriterion(CriterionName.TableIsClear, false);
        }
        if(col.TryGetComponent(out WeldDetail detail))
        {
            if (WeldProcess.Instance.DetailIsComplete)
            {
                WeldProcess.Instance.SetCriterion(CriterionName.DetailOnTable, true);      
            }
            _resultsUI.SetActive(true);
        }
    }
}
