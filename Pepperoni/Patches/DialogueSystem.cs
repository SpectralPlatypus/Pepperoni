using MonoMod;

#pragma warning disable CS0626, CS0414
namespace Pepperoni.Patches
{
    [MonoModPatch("global::DialogueSystem")]
    class DialogueSystem : global::DialogueSystem
    {
        [MonoModIgnore] private bool SetCostumePortraits;
        private portrait[] noidPortraits;

        private extern void orig_Start();
        private void Start()
        {
            noidPortraits = new portrait[5];
            for (int k = 0; k < 4; k++)
            {
                noidPortraits[k] = Portraits[k];
            }
            // eye blink?
            noidPortraits[4] = Portraits[7];

            orig_Start();
        }
        private extern void orig_ParseScriptFile(string Text);
        private void ParseScriptFile(string Text)
        {
            Text = ModHooks.Instance.OnParseScript(Text);
            orig_ParseScriptFile(Text);
        }

        /// <summary>
        /// Should be invoked if the mod changes character type to update the dialogue portrait
        /// </summary>
        public void UpdateCostumePortrait()
        {
            if (PlayerMachine.CurrentCostume == Costumes.Default)
            {
                for (int k = 0; k < 4; k++)
                {
                    Portraits[k] = noidPortraits[k];
                }
                Portraits[7] = noidPortraits[4];
            }
            else
            {
                SetCostumePortraits = false;
            }
        }
    }
}
