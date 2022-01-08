using System;
using System.Collections;
using System.Collections.Generic;
using ReducedBox2D;
using UnityEngine;

public static class Utils
{
    public static void LogBox(Vector2 pos, Vector2 siz, float rot, float time, Color c)
    {
        foreach (var v in GetBoxDrawLine(pos, siz, rot))
        {
            Debug.DrawLine(v.Item1, v.Item2, c, time);
        }
    }

    public static List<Tuple<Vector2, Vector2>> GetBoxDrawLine(Vector2 pos, Vector2 siz, float rot)
    {
        var p = siz / 2f;
        var ps = new[] {p, new Vector2(-p.x, p.y), -p, new Vector2(p.x, -p.y)};
        for (int i = 0; i < ps.Length; i++)
        {
            ps[i] = ps[i].Rotate(rot);
        }

        var ans = new List<Tuple<Vector2, Vector2>>();
        for (int i = 0; i < ps.Length; i++)
        {
            ans.Add(new Tuple<Vector2, Vector2>(ps[i] + pos, ps[(i + 1) % 4] + pos));
        }

        return ans;
    }
}