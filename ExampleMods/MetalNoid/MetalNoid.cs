using Pepperoni;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MetalNoid
{
    public class MetalNoid : Mod
    {

        private Texture2D metalTexture = null;

        public MetalNoid() : base("MetalNoidMod")
        {
        }

        public override string GetVersion() => "1.2";

        public override void Initialize()
        {
            try
            {
                var fileName = Assembly.GetExecutingAssembly().GetManifestResourceNames()[0];
                using (Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
                {
                    byte[] imageBuffer = new byte[imageStream.Length];
                    imageStream.Read(imageBuffer, 0, imageBuffer.Length);
                    imageStream.Flush();
                    metalTexture = new Texture2D(1, 1);
                    metalTexture.LoadImage(imageBuffer);
                    LogDebug("Loaded Metal Texture");
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
            if (PlayerMachine.CurrentCostume != Costumes.Default) return;

            var mat = skinnedMesh.material;
            mat.shader = Shader.Find("Standard");
            mat.DisableKeyword("_METALLICGLOSSMAP");
            mat.SetFloat("_Mode", 0f);
            mat.SetTexture("_MainTex", metalTexture);
            mat.SetFloat("_Metallic", 0.7f);
            mat.SetFloat("_Glossiness", 0.55f);
            
        }

        public void OnEarlyUpdate(PlayerMachine playerMachine)
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
    }
}
