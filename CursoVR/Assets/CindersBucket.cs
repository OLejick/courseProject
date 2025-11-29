using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CindersBucket : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if(col.TryGetComponent(out Cinders cinders))
        {
            WeldProcess.Instance.SetCriterion(CriterionName.CindersRemoved, true);
        }
    }
}
