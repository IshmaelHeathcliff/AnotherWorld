using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "newEvents", menuName = "ScriptObjects/Events/New Events")]
    public class CellEvents : SerializedScriptableObject
    {
        [OdinSerialize] public List<Event> Events;

        public void Execute()
        {
            var id = Random.Range(0, Events.Count);
            Events[id].Execute();
        }
    }
}
