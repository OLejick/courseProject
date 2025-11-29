using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHands : XRBaseController
{
    public void ChangeHands(Transform _newHands)
    {
        modelPrefab = _newHands;
        UpdateController();
    }
}
