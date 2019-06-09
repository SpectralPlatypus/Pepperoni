using MonoMod;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

#pragma warning disable CS0108, CS0626, CS0114, CS0169, CS0649
namespace Pepperoni.Patches
{
    [MonoModPatch("global::PlayerMachine")]
    public class PlayerMachine : global::PlayerMachine
    {
        public delegate IEnumerator WaitFuncDelegate();

        [MonoModIgnore] private SkinnedMeshRenderer SkinnedMesh;
        [MonoModIgnore] private Vector3 LastGround;

        public Vector3 LastGroundLoc { get { return LastGround; } }

        // Default of false also acts as a High-Z state
        public bool? CoyoteFrameEnabled { get; set; }

        public extern void orig_UnLoad();
        public void UnLoad()
        {
            orig_UnLoad();
            ModHooks.Instance.OnPlayerUnLoad();
        }

        [MonoModIgnore]
        [PatchJumpSuperUpdate]
        private extern void Jump_SuperUpdate();

        public extern void orig_SetCostume(Costumes costume);
        public void SetCostume(Costumes costume)
        {
            orig_SetCostume(costume);
            ModHooks.Instance.OnPlayerSetCostume(SkinnedMesh);
        }

        protected extern void orig_EarlyGlobalSuperUpdate();
        protected void EarlyGlobalSuperUpdate()
        {
            orig_EarlyGlobalSuperUpdate();
            ModHooks.Instance.OnPlayerEarlyUpdate(this);
        }

        public static bool _IsCoyoteFrameEnabled(bool value, global::PlayerMachine self)
            => (self as PlayerMachine).IsCoyoteFrameEnabled(value);

        public bool IsCoyoteFrameEnabled(bool value)
            => CoyoteFrameEnabled ?? value;

        public void RunCoroutine(WaitFuncDelegate waitFunc)
        {
            StartCoroutine(waitFunc());
        }
    }
}
