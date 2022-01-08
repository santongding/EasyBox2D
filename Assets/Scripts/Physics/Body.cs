using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReducedBox2D
{
    public class Body
    {
        private List<Box> _boxs = new List<Box>();
        private Vector2 _velocity = Vector2.zero;


        private Vector2 _massCenter = Vector2.zero;
        private float _angle = 0f;
        private float _invMass = 0;
        private float _invI = 0;
        private float _angleVelocity = 0f;


        private float _torgue = 0f;


        private Vector2 _force = Vector2.zero;

        public float InvMass => _invMass;
        public float InvI => _invI;

        public Vector2 Pos
        {
            get => _massCenter;
            set
            {
                var delta = value - _massCenter;
                _massCenter += delta;
                foreach (var box in _boxs)
                {
                    box.pos += delta;
                }
            }
        }

        public float Rot
        {
            get => _angle;
            set
            {
                var delta = value - _angle;
                _angle += delta;
                foreach (var box in _boxs)
                {
                    box.Rot += _angle;
                    var r = box.pos - _massCenter;
                    box.pos = _massCenter + r.Rotate(delta);
                }
            }
        }

        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public float AngleVelocity
        {
            get => _angleVelocity;
            set => _angleVelocity = value;
        }

        public float Torgue
        {
            get => _torgue;
            set => _torgue = value;
        }

        public Vector2 Force
        {
            get => _force;
            set => _force = value;
        }

        public Body(List<Box> boxs)
        {
            foreach (var box in boxs)
            {
                AddBox(box);
            }
        }

        internal void AddBox(Box b)
        {
            _boxs.Add(b);
            if (_invMass == 0 || b.mass == float.PositiveInfinity)
            {
                _invMass = 0;
                _invI = 0;
                _massCenter = Vector2.positiveInfinity;
            }
            else if (_boxs.Count == 1)
            {
                _invMass = b.mass;
                _invI = b.InvI;
                _massCenter = b.pos;
            }
            else
            {
                _massCenter = (_massCenter / _invMass + b.mass * b.pos);
                _invMass = 1f / (1f / _invMass + b.mass);

                _massCenter *= _invMass;
                ReCalcInvI();
            }
        }

        internal bool TryRemoveBox(Box b)
        {
            if (_boxs.Contains(b))
            {
                _boxs.Remove(b);

                if (_boxs.Count == 0)
                {
                    _invI = 0f;
                    _invMass = 0f;
                }
                else if (_boxs.Exists(s => s.mass == float.PositiveInfinity))
                {
                    _invI = 0f;
                    _invMass = 0f;
                    Debug.Assert(_massCenter == Vector2.positiveInfinity);
                }
                else
                {
                    _massCenter = (_massCenter / _invMass - b.mass * b.pos);
                    _invMass = 1f / (1f / _invMass - b.mass);
                    _massCenter *= _invMass;
                    ReCalcInvI();
                }

                return true;
            }

            return false;
        }

        public void ClearAllBox()
        {
            _boxs.Clear();
        }

        public IEnumerable<Box> BoxIterator => _boxs;

        internal void ReCalcInfo()
        {
            _invMass = 0f;
            _massCenter = Vector2.zero;
            foreach (var box in _boxs)
            {
                _invMass += box.mass;
                _massCenter += box.pos * box.mass;
            }

            _invMass = 1f / _invMass;
            _massCenter *= _invMass;

            ReCalcInvI();
        }

        private void ReCalcInvI()
        {
            _invI = 0f;
            foreach (var box in _boxs)
            {
                _invI += 1f / box.InvI + box.mass * (box.pos - _massCenter).sqrMagnitude;
            }

            _invI = 1f / _invI;
        }
    }

    public class Box
    {
        internal Vector2 pos;
        internal Vector2 Siz;
        public float Rot; //in rand
        internal float mass;

        public Vector2 Pos
        {
            get => pos;
            set
            {
                if (pos.Equals(value)) return;
                else
                {
                    pos = value;
                    _body.ReCalcInfo();
                }
            }
        }


        public float Mass
        {
            get => mass;
            set
            {
                if (mass.Equals(value)) return;
                else
                {
                    mass = value;
                    _body.ReCalcInfo();
                }
            }
        }


        public float Friction;
        private Body _body;

        public Box(Vector2 p, Vector2 s, float r, float m, float f)
        {
            pos = p;
            Siz = s;
            Rot = r;
            mass = m;
            Friction = f;
        }

        public void SetBody(Body b)
        {
            if (_body == b)
            {
                return;
            }
            else
            {
                if (_body != null)
                {
                    Debug.Assert(_body.TryRemoveBox(this));
                }

                _body = b;
                if (b != null)
                    b.AddBox(this);
            }
        }

        public Body Body => _body;
        public float InvI => 12f / (mass * Siz.sqrMagnitude);
    }
}