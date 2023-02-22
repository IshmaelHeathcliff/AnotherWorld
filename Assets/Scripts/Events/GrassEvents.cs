using System;
using UnityEngine;

namespace Events
{
    public class GrassEvent0 : Event
    {
        public override void Execute()
        {
            // Debug.Log("grass arrived");
        }
    }
    
    public class GrassEvent1 : Event
    {
        public override void Execute()
        {
            // Debug.Log("another grass arrived");
        }
    }
}
