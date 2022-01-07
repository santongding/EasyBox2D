using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace EasyBox2D
{
    public class Arbiter
    {
        public List<BoxArbiter> arbiters = new List<BoxArbiter>();
        private Body _ra, _rb;

        public Arbiter(Body ra,Body rb)
        {
            _ra = ra;
            _rb = rb;
        }
        public class BoxArbiter
        {
            private bool rev;//是否反转ra和rb
            private List<Contact> _contacts = new List<Contact>();
        }
    }
    
}