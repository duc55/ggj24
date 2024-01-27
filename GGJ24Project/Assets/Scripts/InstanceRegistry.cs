using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam
{
    public class InstanceRegistry<T> where T: Component
    {
        private static HashSet<T> s_instances;
        private static HashSet<T> InstancesNoNull
            => s_instances ??= new HashSet<T>();
        public static IEnumerable<T> All => InstancesNoNull;

        public delegate void AddEvent(T instance);
        public static AddEvent OnAdd;
        public delegate void RemoveEvent(T instance);
        public static RemoveEvent OnRemove;
        
        public static void Add(in T instance)
        {
            InstancesNoNull.Add(instance);
            OnAdd?.Invoke(instance);
        }
        public static void Remove(in T instance)
        {
            InstancesNoNull.Remove(instance);
            OnRemove?.Invoke(instance);
        }

        public static void Clear()
            => InstancesNoNull.Clear();
    }
}