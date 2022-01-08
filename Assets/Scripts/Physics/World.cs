using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReducedBox2D
{
    public class World
    {
        private Vector2 _gravity;

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

        public List<Arbiter>  Simulate(float deltaTime)
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
                body.Velocity += deltaTime * (_gravity + body.InvMass * body.Force);
                body.AngleVelocity += deltaTime * (body.InvMass * body.Torgue);
            }
            
            foreach (var arbiter in arbiters)
            {
                arbiter.PreStep(deltaTime);
            }
            
            foreach (var arbiter in arbiters)
            {
                arbiter.ApplyImpulse(deltaTime);
            }
            
            UpdatePos(deltaTime);
            
            
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