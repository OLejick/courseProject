using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rug : MonoBehaviour
{
    public void SetRug()
    {
        GetComponent<Collider>().enabled = false;
        if(!WeldProcess.Instance.DetailIsComplete)
            WeldProcess.Instance.SetCriterion(CriterionName.Rug, true);
    }
}
