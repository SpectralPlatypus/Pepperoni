using Pepperoni;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace FastGreen
{
    public class FastGreen : Mod
    {

        private Texture2D greenTexture = null;
        public FastGreen() : base("FastGreen")
        {
        }

        public override string GetVersion() => "1.0";

        public override void Initialize()
        {
            string fileName = "";
            foreach (string fn in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (fn.Contains("green"))
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
                    greenTexture = new Texture2D(1, 1);
                    greenTexture.LoadImage(imageBuffer);
                    LogDebug("Loaded Green Texture");
                    ModHooks.Instance.OnPlayerSetCostumeHook += OnSetCostume;
                    ModHooks.Instance.OnPlayerEarlyUpdateHook += OnEarlyUpdate;
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void OnSetCostume(SkinnedMeshRenderer skinnedMesh)
        {
            if (PlayerMachine.CurrentCostume == Costumes.Fast)
            {
                skinnedMesh.material.SetTexture("_MainTex", greenTexture);
            }
        }

        public void OnEarlyUpdate(PlayerMachine playerMachine)
        {
            if (PlayerMachine.CurrentCostume == Costumes.Fast)
            {
                playerMachine.FrictionMult = 0.5f;
            }
            else
            {
                playerMachine.FrictionMult = 1;
            }
        }
    }
}
