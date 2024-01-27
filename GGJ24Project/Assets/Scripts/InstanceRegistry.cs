using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam
{
    public static class InstanceRegistry<TOwner, TInstance> 
        where TOwner: Component
        where TInstance: Component
    {
        private static Dictionary<TInstance, TOwner> s_instanceOwners;
        private static Dictionary<TInstance, TOwner> InstanceOwners 
            => s_instanceOwners ??= new Dictionary<TInstance, TOwner>();

        public static void Register(in TOwner owner, in TInstance instance)
        {
            if (!instance)
            {
                Debug.LogError($"Instance is null. Can't register this!");
                return;
            }
            if (!owner)
            {
                Debug.LogError($"Owner is null. Can't register this!");
                return;
            }

            if (InstanceOwners.ContainsKey(instance))
            {
                Debug.LogWarning($"{instance.name} already has owner: {s_instanceOwners[instance]}. " +
                                 $"Overwriting with new owner, {owner.name}");
                s_instanceOwners[instance] = owner;
            }
            else
            {
                Debug.Log($"Registering {owner.name} as {instance.name}'s owner.");
                s_instanceOwners.Add(instance, owner);
            }
        }

        public static void Clear()
            => InstanceOwners.Clear();

        public static void Remove(in TInstance instance)
            => InstanceOwners.Remove(instance);

        public static bool TryGetOwner(in TInstance instance, out TOwner owner)
            => InstanceOwners.TryGetValue(instance, out owner);

        public static bool TryGetOwnerOrRegister(in TInstance instance, out TOwner owner)
        {
            if (TryGetOwner(instance, out owner))
                return true;

            owner = instance.GetComponentInParent<TOwner>();
            if (owner)
            {
                Register(owner, instance);
                return true;
            }
            
            Debug.LogWarning($"Failed to find a parent of type {typeof(TOwner)} for " +
                             $"{typeof(TInstance)} {instance.name}.");
            return false;
        }
    }
}