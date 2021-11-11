using Pepperoni;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MetalNoid
{
    public class MetalNoid : Mod
    {

        private Texture2D metalTexture = null;
        public MetalNoid() : base("MetalNoidMod")
        {
        }

        public override string GetVersion() => "1.6";

        public override void Initialize()
        {
            string fileName = "";
            foreach (string fn in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (fn.Contains("Noid_gold"))
                {
                    fileName = fn;
                    break;
                }
            }

            try
            {
                if (fileName == "")
                    throw new FileNotFoundException();

                using (Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
                {
                    byte[] imageBuffer = new byte[imageStream.Length];
                    imageStream.Read(imageBuffer, 0, imageBuffer.Length);
                    imageStream.Flush();
                    metalTexture = new Texture2D(1, 1);
                    metalTexture.LoadImage(imageBuffer);
                    LogDebug("Loaded Metal Texture");
                    ModHooks.Instance.OnPlayerSetCostumeHook += OnSetCostume;
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        void OnSetCostume(SkinnedMeshRenderer skinnedMesh)
        {
            if (PlayerMachine.CurrentCostume != Costumes.Default) return;
            var mats = Resources.FindObjectsOfTypeAll<Material>();
            foreach (var m in mats)
            {
                if (m.name.Contains("Complete"))
                {
                    LogDebug("Found material!");
                    skinnedMesh.material = m;
                    skinnedMesh.material.SetTexture("_MainTex", metalTexture);
                }
            }
        }

        /*
        void OnEarlyUpdate(PlayerMachine playerMachine)
        {
            if (PlayerMachine.CurrentCostume == Costumes.Default)
            {
                playerMachine.FrictionMult = 2;
            }
            else
            {
                playerMachine.FrictionMult = 1;
            }
        }
        */
    }
}
