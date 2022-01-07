using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBox2D
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

        public void Simulate(float deltaTime)
        {
            var arbiters = new List<Arbiter>();
            for (int i = 0; i < _bodies.Count - 1; i++)
            {
                for (int j = i + 1; j < _bodies.Count; j++)
                {
                    arbiters.AddRange(CollideUtil.GetArbiter(_bodies[i], _bodies[j]));
                }
            }
        }
    }
}