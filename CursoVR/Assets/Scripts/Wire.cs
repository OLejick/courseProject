using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private List<Transform> _segments;

    public bool IsHolder;
    public bool IsDetail;

    public DetailWire DetailWire;

    private void Awake()
    {
        _lineRenderer.positionCount = _segments.Count;
    }

    private void Update()
    {
        for (int i = 0; i < _segments.Count; i++)
        {
            _lineRenderer.SetPosition(i, _segments[i].transform.position);
        }     
    }

    public void CheckIntegrity()
    {
        WeldProcess.Instance.SetCriterion(CriterionName.WireChecked, true);
    }
}
