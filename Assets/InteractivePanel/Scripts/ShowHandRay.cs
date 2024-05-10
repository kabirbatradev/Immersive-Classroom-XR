using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class ShowHandRay : MonoBehaviour
{
    [SerializeField] private RayInteractor _rayInteractor;
    [SerializeField] private Shapes.Line _lineRenderer;
    public Color lineColorNormal;
    public Color lineColorSelected;
    
    void Update()
    {
        if (_rayInteractor.State == InteractorState.Disabled)
        {
            _lineRenderer.gameObject.SetActive(false);
            return;
        }

        if (_rayInteractor.CollisionInfo == null)
        {
            _lineRenderer.gameObject.SetActive(false);
            return;
        }

        if (!_lineRenderer.gameObject.activeSelf)
        {
            _lineRenderer.gameObject.SetActive(true);
        }
        
        float gapDistance = Vector3.Distance(_rayInteractor.Origin, _rayInteractor.End) / 10;
        _lineRenderer.Start = _lineRenderer.transform.InverseTransformPoint(_rayInteractor.Origin + gapDistance * _rayInteractor.Forward);
        _lineRenderer.End = _lineRenderer.transform.InverseTransformPoint(_rayInteractor.End - gapDistance * _rayInteractor.Forward);
        
        if (_rayInteractor.State == InteractorState.Select)
        {
            _lineRenderer.Color = lineColorSelected;
        }
        else
        {
            _lineRenderer.Color = lineColorNormal;
        }
    }
}
