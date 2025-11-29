using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WeldDetail : MonoBehaviour
{
    [SerializeField] private List<GameObject> seams;

    [SerializeField] private bool _detailIsComplete;
    [SerializeField] private bool _massConnected;
    [SerializeField] private bool _mass;
    [SerializeField] private bool _detailConstructed;

    [SerializeField] private WeldDevice _weldDevice;

    [SerializeField] private XRSocketInteractor _plugSocket;

    [SerializeField] private GameObject _cindersPrefab; 
    [SerializeField] private Transform _cindersSpawnPoint; 

    public void ConnectMass(bool mass)
    {
        _massConnected = mass;

        if (mass)
        {
            WeldProcess.Instance.SetCriterion(CriterionName.Grounding, true);
        }

        if (_weldDevice.IsOn)
        {
            _mass = true;
        }

        SetCanWeld();
    }

    public void EnableMass()
    {
        if (_massConnected)
        {
            _mass = true;
            SetCanWeld();
        }
            
    }

    public void DisableMass()
    {
        _mass = false;

        foreach (GameObject seam in seams)
        {
            seam.GetComponent<Collider>().enabled = false;
        }
    }

    public void SetDetailConstructed()
    {
        _detailConstructed = true;
        SetCanWeld();
    }

    public void SetCanWeld()
    {
        if(_mass && _detailConstructed)
        {
            foreach (GameObject seam in seams)
            {
                seam.SetActive(true);
                seam.GetComponent<Collider>().enabled = true;
            }
        }       
    }

    public void CheckSeam()
    {
        int countOfSeam = 0;

        foreach (var seam in seams)
        {
            if(seam.GetComponent<MeshRenderer>().enabled)
                countOfSeam++;
            print(countOfSeam);
        }

        if(countOfSeam == seams.Count)
        {
            if (!_detailIsComplete)
            {
                SpawnCinders();
            }

            _detailIsComplete = true;
            WeldProcess.Instance.DetailIsComplete = true;
            
        }
    }

    public void SpawnCinders()
    {
        Instantiate(_cindersPrefab, _cindersSpawnPoint.transform.position, Quaternion.identity);
    }
}
