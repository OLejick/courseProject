using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireGenerator : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private Transform _startSegment;

    [SerializeField] private GameObject _segmentPrefab;

    [SerializeField] private List<Transform> _segments;

    [SerializeField] private float _distanceBetweenSegments;

    [SerializeField] private int _countSegments;

    [SerializeField] private bool _wireSpawned;

    private void Awake()
    {
        _lineRenderer.positionCount = _countSegments;
        _segments.Add(_startSegment);

        for (int i = 1; i < _countSegments; i++)
        {
            var s = Instantiate(_segmentPrefab, new Vector3(_startSegment.transform.position.x, _startSegment.transform.position.y - _distanceBetweenSegments * i, _startSegment.transform.position.z), Quaternion.identity);
            var hj = s.GetComponent<HingeJoint>();
            hj.connectedBody = _segments[i - 1].GetComponent<Rigidbody>();
            s.GetComponent<Rigidbody>().mass = _countSegments - i;
            _segments.Add(s.transform);
            _lineRenderer.SetPosition(i, s.transform.position);
        }

        _wireSpawned = true;
    }

    private void Update()
    {
        if (_wireSpawned)
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                _lineRenderer.SetPosition(i, _segments[i].transform.position);
            }
        }
    }
}
