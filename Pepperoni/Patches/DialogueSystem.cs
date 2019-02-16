using MonoMod;

#pragma warning disable CS0626, CS0414
namespace Pepperoni.Patches
{
    [MonoModPatch("global::DialogueSystem")]
    class DialogueSystem : global::DialogueSystem
    {
        [MonoModIgnore] private bool SetCostumePortraits;

        private extern void orig_ParseScriptFile(string Text);
        private void ParseScriptFile(string Text)
        {
            Text = ModHooks.Instance.OnParseScript(Text);
            orig_ParseScriptFile(Text);
        }

        /// <summary>
        /// Should be invoked if the mod changes character type to update the dialogue portrait
        /// </summary>
        public void UpdateCostumePortrait() => SetCostumePortraits = false;
    }
}
