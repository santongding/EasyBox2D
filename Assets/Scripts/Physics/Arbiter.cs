using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace ReducedBox2D
{
    public class Arbiter
    {
        public Contact[] Contacts = null;
        private Box _a, _b;

        public Box A => _a;

        public Box B => _b;

        public Arbiter(Box a, Box b, Contact[] cts)
        {
            _a = a;
            _b = b;
            Contacts = cts;
        }


        public void PreStep(float deltaTime)
        {
            var _ra = _a.Body;
            var _rb = _b.Body;
            for (int i = 0; i < Contacts.Length; i++)
            {
        
                var r1 = Contacts[i].Pos - _ra.Pos;
                var r2 = Contacts[i].Pos - _rb.Pos;
                var rn1 = Vector2.Dot(r1, Contacts[i].Normal);
                var rn2 = Vector2.Dot(r2, Contacts[i].Normal);
                var kNormal = _ra.InvMass + _rb.InvMass + _ra.InvI * (r1.sqrMagnitude - rn1 * rn1) +
                              _rb.InvI * (r2.sqrMagnitude - rn2 * rn2);
                Contacts[i].MassNormal = 1 / kNormal;

                var tangent = Contacts[i].Normal.Cross(1.0f);
                var rt1 = Vector2.Dot(r1, tangent);
                var rt2 = Vector2.Dot(r2, tangent);

                var kTangent = _ra.InvMass + _rb.InvMass + _ra.InvI * (r1.sqrMagnitude - rt1 * rt1) +
                               _rb.InvI * (r2.sqrMagnitude - rt2 * rt2);

                Contacts[i].MassTangent = 1 / kTangent;
            }
        }

        public void ApplyImpulse(float deltaTime)
        {
            var ra = _a.Body;
            var rb = _b.Body;
            for (int i = 0; i < Contacts.Length; i++)
            {
                var r1 = Contacts[i].Pos - ra.Pos;
                var r2 = Contacts[i].Pos - rb.Pos;
                var dv = ra.Velocity - rb.Velocity + ra.AngleVelocity.Cross(r1) - rb.AngleVelocity.Cross(r2);
                var dpn = Mathf.Max(0, -dv.Cross( Contacts[i].Normal) * Contacts[i].MassNormal);
                var pn = dpn * Contacts[i].Normal;
                ra.Velocity -= pn * ra.InvMass;
                rb.Velocity += pn * rb.InvMass;
                ra.AngleVelocity -= r1.Cross(pn) * ra.InvI;
                rb.AngleVelocity += r1.Cross(pn) * rb.InvI;
                
                dv = ra.Velocity - rb.Velocity + ra.AngleVelocity.Cross(r1) - rb.AngleVelocity.Cross(r2);
                var u = _a.Friction * _b.Friction;
                var tangent = Contacts[i].Normal.Cross(1.0f);
                var dpt = Mathf.Clamp(-dv.Cross(tangent)*Contacts[i].MassTangent, -u * dpn, u * dpn);
                var pt = dpt * tangent;

                ra.Velocity -= pt * ra.InvMass;
                rb.Velocity += pt * rb.InvMass;
                ra.AngleVelocity -= r1.Cross(pt) * ra.InvI;
                rb.AngleVelocity += r1.Cross(pt) * rb.InvI;

                Contacts[i].Impulse = pn + pt;
            }
        }
    }
}