using MonoMod;

#pragma warning disable CS0626, CS0649
namespace Pepperoni.Patches
{
    [MonoModPatch("global::TitleScreen")]
    public partial class TitleScreen : global::TitleScreen
    {
        [MonoModIgnore] private float ConfirmTime;

        private extern void orig_FixedUpdate();
        private void FixedUpdate()
        {
            float prevConfirm = ConfirmTime;
            orig_FixedUpdate();
            if (ConfirmTime > 0f && prevConfirm == -1f)
            {
                ModHooks.Instance.OnNewGameStart();
            }
        }
    }
}
