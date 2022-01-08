using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReducedBox2D
{
    public class World
    {
        private Vector2 _gravity;
        private int _iterationNum = 10;
        private static float _biasFactor = 0.3f;
        private static float _biasSlot = 0.01f;

        public static float BiasFactor => _biasFactor;

        public static float BiasSlot => _biasSlot;

        private List<Body> _bodies = new List<Body>();

        public World(Vector2 gravity)
        {
            _gravity = gravity;
        }

        public void AddBody(Body body)
        {
            _bodies.Add(body);
        }

        public void RemoveBody(Body body)
        {
            _bodies.Remove(body);
        }

        public List<Arbiter> Simulate(float deltaTime)
        {
            var arbiters = new List<Arbiter>();
            for (int i = 0; i < _bodies.Count - 1; i++)
            {
                for (int j = i + 1; j < _bodies.Count; j++)
                {
                    arbiters.AddRange(Collide.GetArbiter(_bodies[i], _bodies[j]));
                }
            }

            foreach (var body in _bodies)
            {
                if (body.InvMass == 0f)
                {
                    continue;
                }

                body.Velocity += deltaTime * (_gravity + body.InvMass * body.Force);
                body.AngleVelocity += deltaTime * (body.InvI * body.Torgue);
            }

            foreach (var arbiter in arbiters)
            {
                arbiter.PreStep(deltaTime);
            }

            for (int i = 0; i < _iterationNum; i++)
                foreach (var arbiter in arbiters)
                {
                    arbiter.ApplyImpulse(deltaTime);
                }

            UpdatePos(deltaTime);
            //Debug.Log(arbiters.Count);

            return arbiters;
        }

        public void UpdatePos(float delatTime)
        {
            foreach (var body in _bodies)
            {
                body.Pos = body.Pos + body.Velocity * delatTime;
                body.Rot = body.Rot + body.AngleVelocity * delatTime;
                body.Force = Vector2.zero;
                body.Torgue = 0f;
            }
        }
    }
}