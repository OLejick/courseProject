using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireObject : MonoBehaviour
{
    public List<WireObject> WireObjects;

    public void SelectWire()
    {
        foreach (var wire in WireObjects)
        {
            if(wire != this)
            {
                wire.SetSelected(true);
            }
            else
            {
                wire.SetSelected(false);
            }
        }
    }

    public void SetSelected(bool select)
    {
        GetComponent<MeshRenderer>().enabled = select;
        GetComponent<Collider>().enabled = select;
    }
}
