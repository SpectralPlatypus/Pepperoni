using MonoMod;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#pragma warning disable CS0649, CS0108, CS0626
namespace Pepperoni.Patches
{
    [MonoModPatch("global::DebugManager")]
    class DebugManager : global::DebugManager
    { 
        private extern void orig_Start();

        private void Start()
        {
            orig_Start();
            ModLoader.LoadMods();
        }
    }
}
