using System;
using UnityEngine;

namespace Events
{
    [Serializable]
    public abstract class Event
    {
        public abstract void Execute();
    }
}
