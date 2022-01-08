using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReducedBox2D
{
    public static class Collide
    {
        public static List<Arbiter> GetArbiter(Body ra, Body rb)
        {
            var ans = new List<Arbiter>();
            foreach (var ba in ra.BoxIterator)
            {
                foreach (var bb in rb.BoxIterator)
                {
                    if (getContacts(ba, bb) is Arbiter ar)
                    {
                        ans.Add(ar);
                    }
                }
            }

            return ans;
        }

        static Arbiter getContacts(Box a, Box b)
        {
            Mat22 rotA = new Mat22(a.Rot), rotB = new Mat22(b.Rot); //local to world
            Mat22 rotAT = rotA.Transpose, rotBT = rotB.Transpose;
            Vector2 hA = a.Siz / 2f, hB = b.Siz / 2f;

            Vector2 dA = rotAT * (b.pos - a.pos);


            Vector2 faceA = dA.Abs() - hA - (rotAT * rotB).Abs * hB;
            if (faceA.x >= 0 || faceA.y >= 0)
            {
                return null;
            }

            Vector2 dB = rotBT * (a.pos - b.pos);
            Vector2 faceB = dB.Abs() - hB - (rotBT * rotA).Abs * hA;

            if (faceB.x >= 0 || faceB.y >= 0)
            {
                return null;
            }

            bool isA = true;

            float relativeTol = 0.95f;
            float absoluteTol = 0.01f;
            float separation = faceA.x;
            float sideH = hA.y;
            float frontH = hA.x;
            Vector2 sideNormal = rotA.col2;
            Vector2 normal = dA.x > 0 ? rotA.col1 : -rotA.col1;
            if (faceA.y > separation * relativeTol + absoluteTol * hA.y)
            {
                separation = faceA.y;
                normal = dA.y > 0 ? rotA.col2 : -rotA.col2;
                sideNormal = rotA.col1;
                sideH = hA.x;
                frontH = hA.y;
            }

            if (faceB.y > separation * relativeTol + absoluteTol * hB.y)
            {
                separation = faceB.y;
                normal = dB.y > 0 ? rotB.col2 : -rotB.col2;
                sideNormal = rotB.col1;
                isA = false;
                sideH = hB.x;
                frontH = hB.y;
            }

            if (faceB.x > separation * relativeTol + absoluteTol * hB.x)
            {
                separation = faceB.x;
                normal = dB.x > 0 ? rotB.col1 : -rotB.col1;
                sideNormal = rotB.col2;
                isA = false;
                sideH = hB.y;
                frontH = hB.x;
            }

            var cts = computeContactEdge(isA ? b : a, isA ? rotB : rotA, isA ? rotBT : rotAT, normal);
            Debug.Assert(cts.Length == 2);
            var p = isA ? a.pos : b.pos;
            if (clipEdge(cts, sideNormal, sideH, p) == false || clipEdge(cts, -sideNormal, sideH, p) == false)
            {
                return null;
            }

            var contacts = new List<Contact>();

            for (int i = 0; i < cts.Length; i++)
            {
                cts[i].Separation = frontH - Vector2.Dot((cts[i].Pos - p), normal);
                if (cts[i].Separation <= -1e-4)
                {
                    continue;
                }

                cts[i].Normal = normal;
                contacts.Add(cts[i]);
            }
            Debug.Assert(contacts.Count > 0);
            return isA ? new Arbiter(a, b, contacts.ToArray()) : new Arbiter(b, a, contacts.ToArray());
        }

        private static Vector2[] fx = new[]
            {new Vector2(1, 1), new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, -1)};

        static Contact[] computeContactEdge(Box b, Mat22 rot, Mat22 rotT, Vector2 normal)
        {
            var localNormal = rotT * (-normal);
            var angle = Mathf.Atan2(localNormal.y, localNormal.x);
            var i = Mathf.FloorToInt((angle + Mathf.PI / 4f) / (Mathf.PI / 2));
            i = ((i % 4) + 4) % 4;
            var j = (i + 3) % 4;
            return new[]
            {
                new Contact {Pos = b.pos + rot * new Vector2(fx[i].x * b.Siz.x, fx[i].y * b.Siz.y) / 2f},
                new Contact {Pos = b.pos + rot * new Vector2(fx[j].x * b.Siz.x, fx[j].y * b.Siz.y) / 2f}
            };
        }

        static bool clipEdge(Contact[] cts, Vector2 sideNormal, float h, Vector2 pos)
        {
            var dis0 = Vector2.Dot(sideNormal, cts[0].Pos - pos) + h;
            var dis1 = Vector2.Dot(sideNormal, cts[1].Pos - pos) + h;
            if (dis0 >= 0 && dis1 >= 0)
            {
                return true;
            }
            else if (dis0 < 0 && dis1 < 0)
            {
                return false;
            }
            else
            {
                if (dis0 < 0)
                {
                    cts[0].Pos = cts[0].Pos + (cts[1].Pos - cts[0].Pos) * (-dis0) / (dis1 - dis0);
                }
                else
                {
                    cts[1].Pos = cts[1].Pos + (cts[0].Pos - cts[1].Pos) * (-dis1) / (dis0 - dis1);
                }

                return true;
            }
        }
    }

    public struct Contact
    {
        public Vector2 Normal;
        public Vector2 Pos;
        public Vector2 Impulse;
        public float Separation;
        internal float MassNormal;
        internal float MassTangent;
        internal float Bias;
    }
}