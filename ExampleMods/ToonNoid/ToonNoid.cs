using Pepperoni;
using System.IO;
using UnityEngine;

namespace ToonNoid
{
    public class ToonNoid : Mod
    {
        Shader toon;

        public ToonNoid() : base("ToonNoid")
        {
        }

        public override string GetVersion() => "1.0";

        public override void Initialize()
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(
                Path.Combine(Application.dataPath, @"Managed\Mods\ToonNoid\Assets\toonShader"));
            foreach (var o in bundle.LoadAllAssets())
            {
                Log(o.GetType().Name);
                if (o.GetType().Name == "Shader")
                {
                    toon = o as Shader;
                    Log("Found Shader: " + toon.name);
                    ModHooks.Instance.OnPlayerSetCostumeHook += OnSetCostume;
                }
            }
        }

        void OnSetCostume(SkinnedMeshRenderer skinnedMesh)
        {
            if (PlayerMachine.CurrentCostume != Costumes.Default) return;
            skinnedMesh.material.shader = toon;
        }
    }
}
