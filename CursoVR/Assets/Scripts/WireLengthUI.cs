using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireLengthUI : MonoBehaviour
{
    public void SelectLength(bool correctLength)
    {
        if (!WeldProcess.Instance.DetailIsComplete)
            WeldProcess.Instance.SetCriterion(CriterionName.WireLengthSetted, correctLength);
    }
}
