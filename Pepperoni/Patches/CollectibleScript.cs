using MonoMod;
using UnityEngine;

#pragma warning disable CS0649, CS0626, CS0108
namespace Pepperoni.Patches
{
    [MonoModPatch("global::CollectibleScript")]
    class CollectibleScript : global::CollectibleScript
    {
        [MonoModIgnore]
        public static int CollectiblesPickedUp;

        private extern void orig_OnTriggerEnter(Collider other);
        private void OnTriggerEnter(Collider other)
        {
            orig_OnTriggerEnter(other);
            ModHooks.Instance.OnGetCollectible(CollectiblesPickedUp, other);
        }
    }

}
