using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBox2D
{
    public class Body
    {
        private List<Box> _boxs = new List<Box>();
        private Vector2 _velocity = Vector2.zero;
        private Vector2 _massCenter = Vector2.zero;
        private float _invMass = 0;
        private float _invI = 0;
        private float _angleVelocity = 0f;
        private float _force = 0f;

        public Body(List<Box> boxs)
        {
            foreach (var box in boxs)
            {
                AddBox(box);
            }
        }

        public void AddBox(Box b)
        {
            _boxs.Add(b);
            if (_invMass == 0 || b.Mass == float.PositiveInfinity)
            {
                _invMass = 0;
                _invI = 0;
                _massCenter = Vector2.positiveInfinity;
            }
            else if (_boxs.Count == 1)
            {
                _invMass = b.Mass;
                _invI = 12 / (b.Mass * b.Siz.sqrMagnitude);
                _massCenter = b.Pos;
            }
            else
            {
                _massCenter = (_massCenter / _invMass + b.Mass * b.Pos);
                _invMass = 1f / (1f / _invMass + b.Mass);

                _massCenter *= _invMass;
                ReCalcInvI();
            }
        }

        public bool TryRemoveBoxAtIndex(int index)
        {
            if (index >= 0 && _boxs.Count > index)
            {
                var b = _boxs[index];
                _boxs.RemoveAt(index);

                if (_boxs.Count == 0)
                {
                    _invI = 0f;
                    _invMass = 0f;
                }
                else if (_boxs.Exists(s => s.Mass == float.PositiveInfinity))
                {
                    _invI = 0f;
                    _invMass = 0f;
                    Debug.Assert(_massCenter == Vector2.positiveInfinity);
                }
                else
                {
                    _massCenter = (_massCenter / _invMass - b.Mass * b.Pos);
                    _invMass = 1f / (1f / _invMass - b.Mass);
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


        private void ReCalcInvI()
        {
            _invI = 0f;
            foreach (var box in _boxs)
            {
                _invI += (box.Mass * box.Siz.sqrMagnitude) / 12 + box.Mass * (box.Pos - _massCenter).sqrMagnitude;
            }

            _invI = 1f / _invI;
        }
    }

    public struct Box
    {
        public Vector2 Pos;
        public Vector2 Siz;
        public float Rot; //in rand
        public float Mass;
        public float Friction;
    }
}