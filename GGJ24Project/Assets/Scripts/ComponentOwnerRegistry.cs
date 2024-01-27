using System.Collections.Generic;
using UnityEngine;

namespace LeftOut.GameJam
{
    public static class ComponentOwnerRegistry<TOwner, TComponent> 
        where TOwner: Component
        where TComponent: Component
    {
        private static Dictionary<TComponent, TOwner> s_componentToOwners;
        private static Dictionary<TComponent, TOwner> ComponentOwners 
            => s_componentToOwners ??= new Dictionary<TComponent, TOwner>();

        public static void Register(in TOwner owner, in TComponent instance)
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

            if (ComponentOwners.ContainsKey(instance))
            {
                Debug.LogWarning($"{instance.name} already has owner: {s_componentToOwners[instance]}. " +
                                 $"Overwriting with new owner, {owner.name}");
                s_componentToOwners[instance] = owner;
            }
            else
            {
                //Debug.Log($"Registering {owner.name} as {instance.name}'s owner.");
                s_componentToOwners.Add(instance, owner);
            }
        }

        public static void Clear()
            => ComponentOwners.Clear();

        public static void Remove(in TComponent instance)
            => ComponentOwners.Remove(instance);

        public static void RemoveAllOwned(in TOwner owner)
        {
            var toRemove = new List<TComponent>();
            foreach (var kvp in ComponentOwners)
            {
                if (ReferenceEquals(kvp.Value, owner))
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var component in toRemove)
            {
                ComponentOwners.Remove(component);
            }
        }

        public static bool TryGetOwner(in TComponent instance, out TOwner owner)
            => ComponentOwners.TryGetValue(instance, out owner);

        public static bool TryGetOwnerOrRegister(in TComponent instance, out TOwner owner)
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
                             $"{typeof(TComponent)} {instance.name}.");
            return false;
        }
    }
}