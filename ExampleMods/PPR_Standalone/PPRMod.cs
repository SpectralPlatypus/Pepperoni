using System;
using System.Collections;
using System.Reflection;
using System.Text;
using Pepperoni;
using UnityEngine;

namespace PPR_Standalone
{
    public class PPRMod : Mod
    {
        FieldInfo screamField = null;
        //CounterHUD _counter;
        public PPRMod() : base("PPR", "1.5")
        {
        }

        private const float zapPercent = 0.07f;
        private const float glitchPercent = 0.02f;
        bool glitchRunning = false;

        public override void Initialize()
        {
            ModHooks.Instance.GetCollectibleHook += OnPepObtain;
            ModHooks.Instance.OnParseScriptHook += OnParseScript;

            screamField = typeof(SoundManifest).GetField("Clip_Voidout", 
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void OnPepObtain(int pepCount, Collider other)
        {
            if (pepCount > 0 && other != null)
            {
                PlayerMachine p = other.GetComponent<PlayerMachine>();
                if (!p.voided && !PlayerMachine.CurrentCostume.Equals(Costumes.Miku))
                {
                    int costumeIndex = Convert.ToInt32(Math.Truncate(UnityEngine.Random.Range(0f, 3.99f)));
                    p.SetCostume((Costumes)costumeIndex);
                    GameObject.FindGameObjectWithTag("Manager").GetComponent<DialogueSystem>().UpdateCostumePortrait();
                    var roll = UnityEngine.Random.value;
                    if(roll <= glitchPercent && !glitchRunning)
                    {
                        glitchRunning = true;
                        p.RunCoroutine(GlitchCoroutine);
                    }
                    else if (UnityEngine.Random.value <= zapPercent)
                    {
                        LogDebug($"{_name}: Zaptime!");
                        p.GetStunned(1.2f);
                    }
                }
            }
        }


        protected IEnumerator GlitchCoroutine()
        {
            LogDebug("Enable Glitch");
            Shader.SetGlobalFloat("_Glitcherino", 0.5f);
            yield return new WaitForSeconds(3.0f);
            LogDebug("Disable glitch");
            Shader.SetGlobalFloat("_Glitcherino", 0.0f);
            glitchRunning = false;
        }

        public string OnParseScript(string text)
        {
            if(text.Contains("Chantro"))
            {
                // "%n9%v1%\r\nScott\r\n%m1%Have some fun with%m0%%s1% %m1%%sD%Miku!!\r\n\r\n%n\r\n";
                StringBuilder sb = new StringBuilder();
                sb.Append(DialogueUtils.EmitNewPassageHeader(DialogPortraits.Mushroom, DialogVoices.Mushroom, "Scott"));
                sb.Append("%m1%You are very cute,").Append(DialogueUtils.PauseBrief).Append(" just like Miku!")
                    .Append(DialogueUtils.PassageEnd);
                sb.Append(DialogueUtils.EmitNewPassageHeader(DialogPortraits.NoidSurprise, DialogVoices.NoidSurprise, "Noid", true));
                sb.Append("%m1%").Append(DialogueUtils.SoundFX(1)).Append("Sure...")
                    .Append(DialogueUtils.PassageEnd);
                sb.Append(DialogueUtils.EmitNewPassageHeader(DialogPortraits.Mushroom, DialogVoices.Mushroom, "Scott"));
                sb.Append("%m1%Have some fun with").Append(DialogueUtils.Pause).Append(" Miku!")
                    .Append(DialogueUtils.DialogEnd);
                return sb.ToString();
            }
            else if((bool)GameObject.Find("Boss Room"))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DialogueUtils.EmitNewPassageHeader(DialogPortraits.Noid4thWall, DialogVoices.Noid4thWall));
                sb.Append("%m1%Let's cut to the chase.").Append(DialogueUtils.DialogEnd);
                return sb.ToString();
            }
            return text;
        }

        public void InfiniteJump(ref bool FallWindowCheck, ref float FallTime)
        {
            FallWindowCheck = true;
            FallTime = 0f;
        }
    }
}
