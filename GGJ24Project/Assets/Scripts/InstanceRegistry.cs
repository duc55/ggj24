using System.Collections.Generic;

namespace LeftOut.GameJam
{
    public static class InstanceRegistry<TInstance, TOwner>
    {
        private static Dictionary<TInstance, TOwner> s_instanceOwners;
        private static Dictionary<TInstance, TOwner> InstanceOwners 
            => s_instanceOwners ??= new Dictionary<TInstance, TOwner>();

        public static void Register(in TInstance instance, in TOwner owner)
            => InstanceOwners[instance] = owner;

        public static void Clear()
            => InstanceOwners.Clear();

        public static void Remove(in TInstance instance)
            => InstanceOwners.Remove(instance);

        public static bool TryGetOwner(in TInstance instance, out TOwner owner)
            => InstanceOwners.TryGetValue(instance, out owner);
    }
}