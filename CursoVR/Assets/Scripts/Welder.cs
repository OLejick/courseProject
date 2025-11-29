using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Welder : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor _headSocketInteractor;
    [SerializeField] private XRSocketInteractor _bodySocketInteractor;
    [SerializeField] private XRSocketInteractor _legsSocketInteractor;

    public void SetHeadDress(bool set)
    {
        if (set)
        {
            _headSocketInteractor.firstInteractableSelected.transform.GetComponent<MeshRenderer>().enabled = false;

            if (_headSocketInteractor.firstInteractableSelected.transform.TryGetComponent(out WeldMask weldMask))
            {
                if (!WeldProcess.Instance.DetailIsComplete)
                    WeldProcess.Instance.SetCriterion(CriterionName.Mask, true);
            }
        }
        else
        {
            _headSocketInteractor.firstInteractableSelected.transform.GetComponent<MeshRenderer>().enabled = true;
            WeldProcess.Instance.SetCriterion(CriterionName.Mask, false);
        }
    }

    public void SetBodyDress(bool set)
    {
        if (set)
        {
            if (_bodySocketInteractor.firstInteractableSelected.transform.TryGetComponent(out WeldBodyDress weldMask))
            {
                if (!WeldProcess.Instance.DetailIsComplete)
                    WeldProcess.Instance.SetCriterion(CriterionName.WeldDress, true);
            }
        }
        else
        {
            WeldProcess.Instance.SetCriterion(CriterionName.WeldDress, false);
        }
    }

    public void SetLegDress(bool set)
    {
        if (set)
        {
            if (_legsSocketInteractor.firstInteractableSelected.transform.TryGetComponent(out WeldLegsDress weldMask))
            {
                if (!WeldProcess.Instance.DetailIsComplete)
                    WeldProcess.Instance.SetCriterion(CriterionName.LegDress, true);
            }
        }
        else
        {
            WeldProcess.Instance.SetCriterion(CriterionName.LegDress, false);
        }
    }
}
