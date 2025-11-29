using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DetailWire : MonoBehaviour
{
    public bool Voltage;

    public WeldDetail Detail;

    public void SetVoltage(bool voltage)
    {
        Voltage = true;

        if(voltage)
        {
            Detail.EnableMass();
        }
        else
        {
            Detail.DisableMass();
        }
    }
}
