using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBox2D
{
    public static class CollideUtil
    {
        public static Arbiter GetArbiter(Body ra, Body rb)
        {
            var ans = new Arbiter(ra,rb);
            foreach (var ba in ra.BoxIterator)
            {
                foreach (var bb in rb.BoxIterator)
                {
                    if (getContacts(ba, bb) is Arbiter.BoxArbiter ar)
                    {
                        ans.arbiters.Add(ar);
                    }
                }
            }

            return null;
        }

        static Arbiter.BoxArbiter getContacts(Box a, Box b)
        {
            Mat22 rotA = new Mat22(a.Rot), rotB = new Mat22(b.Rot); //local to world
            Mat22 rotAT = rotA.Transpose, rotBT = rotB.Transpose;
            Vector2 hA = a.Siz / 2f, hB = b.Siz / 2f;

            Vector2 dA = rotAT * (b.Pos - a.Pos);


            Vector2 faceA = dA.Abs() - hA - (rotAT * rotB).Abs * hB;
            if (faceA.x >= 0 || faceA.y >= 0)
            {
                return null;
            }

            Vector2 dB = rotBT * (a.Pos - b.Pos);
            Vector2 faceB = dB.Abs() - hB - (rotBT * rotA).Abs * hA;

            if (faceB.x >= 0 || faceB.y >= 0)
            {
                return null;
            }

            bool isA = true;

            float relativeTol = 0.95f;
            float absoluteTol = 0.01f;
            float separation = faceA.x;
            Vector2 normal = dA.x > 0 ? rotA.col1 : -rotA.col1;
            if (faceA.y > separation * relativeTol + absoluteTol * hA.y)
            {
                separation = faceA.y;
                normal = dA.y > 0 ? rotA.col2 : -rotA.col2;
            }

            if (faceB.y > separation * relativeTol + absoluteTol * hB.y)
            {
                separation = faceB.y;
                normal = dB.y > 0 ? rotB.col2 : -rotB.col2;
                isA = false;
            }

            if (faceB.x > separation * relativeTol + absoluteTol * hB.x)
            {
                separation = faceB.x;
                normal = dB.x > 0 ? rotB.col1 : -rotB.col1;
                isA = false;
            }

            var cts = computeContactEdge(isA ? b : a, isA ? rotBT : rotAT, normal);
        }

        private static Vector2[] fx = new[]
            {new Vector2(1, 1), new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, -1)};

        static List<Contact> computeContactEdge(Box b, Mat22 rot, Vector2 normal)
        {
            var angle = Mathf.Atan2(normal.y, normal.x);
            var i = Mathf.FloorToInt((angle + Mathf.PI / 4f) / (Mathf.PI / 2));
            i = ((i % 4) + 4) % 4;
            var ans = new List<Contact>();
            ans.Add(new Contact {Pos = new Vector2(fx[i].x * b.Siz.x, fx[i].y * b.Siz.y)});
            i = (i+3)%4;
            ans.Add(new Contact {Pos = new Vector2(fx[i].x * b.Siz.x, fx[i].y * b.Siz.y)});
            return ans;
        }
    }

    public struct Contact
    {
        public Vector2 Normal;
        public Vector2 Pos;
        public float Impluse;
        public float Separation;
    }
}