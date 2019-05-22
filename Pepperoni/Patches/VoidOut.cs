using MonoMod;

#pragma warning disable CS0626, CS0414
namespace Pepperoni.Patches
{
    [MonoModPatch("global::VoidOut")]
    class VoidOut : global::VoidOut
    {
        [MonoModIgnore] private bool active = true;

        /// <summary>
        /// Functionality to toggle deathplanes
        /// </summary>
        public void setActive(bool state) => active = state;
    }
}
