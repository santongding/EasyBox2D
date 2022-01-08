using System;
using System.Collections;
using System.Collections.Generic;
using ReducedBox2D;
using UnityEngine;

[DisallowMultipleComponent]
public class Rigid : MonoBehaviour
{
    private List<Rect> _boxes = null;
    private Body _body;

    public Body Body => _body;

    private Vector2 _offset;
    private float _rotOffset;


    private void Awake()
    {
        _boxes = new List<Rect>(GetComponents<Rect>());
        var bs = new List<Box>();
        foreach (var boxCollider in _boxes)
        {
            bs.Add(boxCollider.Box);
        }

        _body = new Body(bs);
        _offset = _body.Pos - (Vector2) this.transform.position;
        _rotOffset = 0 - this.transform.eulerAngles.z / 180f * Mathf.PI;

        Main.Instance.AddRigid(this);
    }

    public void UpdatePos()
    {
        foreach (var box in _boxes)
        {
            box.UpdatePos();
        }

        transform.position = _body.Pos - _offset.Rotate(_body.Rot);
        transform.eulerAngles = new Vector3(0, 0, (_body.Rot - _rotOffset) / Mathf.PI * 180f);

        //Debug.Log(_body.Rot - this.transform.eulerAngles.z / 180f * Mathf.PI);
    }

    public void SyncInfo()
    {
        _body.Rot = this.transform.eulerAngles.z / 180f * Mathf.PI + _rotOffset;
        _body.Pos = (Vector2) (this.transform.position) + _offset.Rotate(_body.Rot);
    }

    private void OnDrawGizmos()
    {
        if (_body != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_body.Pos, 0.1f);
            Gizmos.DrawLine(_body.Pos, _body.Pos + _body.Velocity);
        }
    }
}