using MonoMod;
using UnityEngine;

#pragma warning disable CS0108, CS0626
namespace Pepperoni.Patches
{
    [MonoModPatch("global::Speaker")]
    class Speaker : global::Speaker
    {
        public extern void orig_PlaySound(AudioClip clip, bool PlayRandom = false);

        public void PlaySound(AudioClip clip, bool PlayRandom = false)
        {
            AudioClip newClip = ModHooks.Instance.OnSpeakerPlay(clip);
            orig_PlaySound(newClip, PlayRandom);
        }
    }
}
