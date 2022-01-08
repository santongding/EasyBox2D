using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReducedBox2D;

public class Main : MonoBehaviour
{
    static Main _instance;
    private World _world = null;
    public static Main Instance => _instance;
    private List<Rigid> _rigids = new List<Rigid>();
    private Dictionary<Box, Rect> _boxs = new Dictionary<Box, Rect>();
    public Vector2 Gravity = new Vector2(0, -9.8f);

    private void Awake()
    {
        Debug.Assert(_instance == null);
        _instance = this;
        _world = new World(Gravity);
    }

    public void AddRigid(Rigid r)
    {
        _rigids.Add(r);
        _world.AddBody(r.Body);
    }

    public void AddBox(Rect b)
    {
        Debug.Assert(b.Box != null);
        _boxs[b.Box] = b;
    }

    private void FixedUpdate()
    {
        foreach (var r in _rigids)
        {
            r.SyncInfo();
        }

        var arbiters = _world.Simulate(Time.fixedDeltaTime);
        foreach (var r in _rigids)
        {
            r.UpdatePos();
        }

        foreach (var ar in arbiters)
        {
            var a = _boxs[ar.A];
            var b = _boxs[ar.B];
            b.OnCollision(a, new List<Contact>(ar.Contacts));
            a.OnCollision(b, new List<Contact>(ar.Contacts).ConvertAll(s =>
            {
                s.Normal = -s.Normal;
                return s;
            }));
        }
    }
}