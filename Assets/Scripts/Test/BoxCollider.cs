using System;
using System.Collections;
using System.Collections.Generic;
using ReducedBox2D;
using UnityEngine;
[RequireComponent(typeof(Rigid))]
public class BoxCollider : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector2 Siz;
    public Vector2 Offset;
    public float Mass;
    public bool IsStatic = false;
    public float Friction;
    ReducedBox2D.Box _box = null;

    public ReducedBox2D.Box Box
    {
        get
        {
            if (_box == null)
            {
                _box = new Box(transform.TransformPoint(Offset), Siz, transform.eulerAngles.z / 180f * Mathf.PI, IsStatic?float.PositiveInfinity:Mass,
                    Friction);
            }

            return _box;
        }
    }

    public void UpdatePos()
    {
        transform.eulerAngles = new Vector3(0, 0, _box.Rot / Mathf.PI * 180f);
        transform.position = _box.Pos - (Vector2) transform.TransformPoint(Offset) + (Vector2) transform.position;
    }
    
    private void OnDrawGizmos()
    {
        var pos = transform.TransformPoint(Offset);
        var rot = transform.eulerAngles.z / 180f * Mathf.PI;
        var p = Siz / 2f;
        var ps = new[] {p, new Vector2(-p.x, p.y), -p, new Vector2(p.x, -p.y)};
        for (int i = 0; i < ps.Length; i++)
        {
            ps[i] = ps[i].Rotate(rot);
        }

        for (int i = 0; i < ps.Length; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(ps[i], ps[(i + 1) % 4]);
        }
    }

    public virtual void OnCollision(BoxCollider collider,List<Contact>contacts)
    {
        foreach (var contact in contacts)
        {
            Gizmos.color = Color.red;
            
            Gizmos.DrawWireSphere(contact.Pos,0.1f);
        }
    }
}