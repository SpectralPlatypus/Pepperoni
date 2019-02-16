using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DustMath;
using MonoMod;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0626, CS0108
namespace Pepperoni.Patches
{
    [MonoModPatch("global::SaveScript")]
    public class SaveScript : global::SaveScript
    {
        public extern static void orig_Save();
        public static void Save()
        {
            ModHooks.Instance.OnSaveGameBeforeSave();
            orig_Save();
            ModHooks.Instance.OnSaveGameAfterSave();
        }
    }
}