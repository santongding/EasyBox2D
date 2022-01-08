using System;
using System.Collections;
using System.Collections.Generic;
using ReducedBox2D;
using UnityEngine;

[DisallowMultipleComponent]
public class Rigid : MonoBehaviour
{
    private List<BoxCollider> _boxes = null;
    private Body _body;

    public Body Body => _body;

    private Vector2 _offset;
    private float _rotOffset;
    
    

    private void Awake()
    {
        _boxes = new List<BoxCollider>(GetComponents<BoxCollider>());
        var bs = new List<Box>();
        foreach (var boxCollider in _boxes)
        {
            bs.Add(boxCollider.Box);
        }

        _body = new Body(bs);
        _offset = _body.Pos - (Vector2) this.transform.position;
        _rotOffset = 0 - this.transform.eulerAngles.z / 180f * Mathf.PI;
    }

    public void UpdatePos()
    {
        transform.position = _body.Pos - _offset;
        transform.eulerAngles = new Vector3(0, 0, (_body.Rot - _rotOffset) / Mathf.PI * 180f);
        foreach (var box in _boxes)
        {
            box.UpdatePos();;
        }
    }

    void SyncInfo()
    {
        _body.Pos = (Vector2) (this.transform.position) + _offset;
        _body.Rot = this.transform.eulerAngles.z / 180f * Mathf.PI + _rotOffset;
    }

    private void LateUpdate()
    {
        SyncInfo();
    }
}