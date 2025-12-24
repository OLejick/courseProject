using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwithcOnLock : MonoBehaviour
{
    public GameObject swithc1;
    public bool isPresed = false;
    
    public void Switch()
    {
        isPresed = !isPresed;
            
        swithc1.SetActive(isPresed);
    }
}