using System;
using System.Collections;
using System.Collections.Generic;
using ReducedBox2D;
using UnityEngine;

[RequireComponent(typeof(Rigid))]
public class Rect : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector2 Siz = Vector2.one;
    public Vector2 Offset;
    public float Mass = 1f;
    public bool IsStatic = false;
    public float Friction = 0.5f;
    ReducedBox2D.Box _box = null;

    public ReducedBox2D.Box Box
    {
        get
        {
            if (_box == null)
            {
                _box = new Box(transform.TransformPoint(Offset), Siz, transform.eulerAngles.z / 180f * Mathf.PI,
                    IsStatic ? float.PositiveInfinity : Mass,
                    Friction);
                Main.Instance.AddBox(this);
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
        var pos = (Vector2) transform.TransformPoint(Offset);
        var rot = transform.eulerAngles.z / 180f * Mathf.PI;


        Gizmos.color = Color.green;


        foreach (var l in Utils.GetBoxDrawLine(pos, Siz, rot))
        {
            Gizmos.DrawLine(l.Item1, l.Item2);
        }

        if (_box != null)
        {
      
            foreach (var l in Utils.GetBoxDrawLine(_box.Pos, _box.Siz, _box.Rot))
            {
                Gizmos.DrawLine(l.Item1, l.Item2);
            }
        }
    }


    public virtual void OnCollision(Rect collider, List<Contact> contacts)
    {
        foreach (var contact in contacts)
        {
            Utils.LogBox(contact.Pos, Vector2.one * 0.1f, 0, Time.fixedDeltaTime, Color.red);
            if (name != "Ground")
            {
                Debug.DrawLine(contact.Pos, contact.Pos + contact.Impulse, Color.red, Time.fixedDeltaTime);
            }
        }
    }
}