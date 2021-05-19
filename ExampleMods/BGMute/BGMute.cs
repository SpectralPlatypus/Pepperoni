using Pepperoni;
using UnityEngine;

namespace BGMute
{
    public class BGMute : Mod
    {
        private const string _modVersion = "1.0";
        // private bool _muted;
        public BGMute() : base("BGMute")
        {
        }

        public override string GetVersion() => _modVersion;

        public override void Initialize()
        {
            On.VoidMusic.Update += VoidMusic_Update;
            On.BossRoomController.BossDialogueEnd += BossRoomController_BossDialogueEnd;
            On.BossRoomController.KeepMusicGoing += BossRoomController_KeepMusicGoing;
            On.TitleScreen.FixedUpdate += TitleScreen_FixedUpdate;
        }

        private void TitleScreen_FixedUpdate(On.TitleScreen.orig_FixedUpdate orig, TitleScreen self)
        {
            orig(self);
            self.Music.volume = 0f;
            self.YoNoidChant.volume = 0f;
        }

        private void VoidMusic_Update(On.VoidMusic.orig_Update orig, VoidMusic self)
        {
            orig(self);
            self.Clear();
        }

        private void BossRoomController_KeepMusicGoing(On.BossRoomController.orig_KeepMusicGoing orig, BossRoomController self)
        {
            // Do nothing for now
        }

        private void BossRoomController_BossDialogueEnd(On.BossRoomController.orig_BossDialogueEnd orig, BossRoomController self)
        {
            orig(self);
            if (BossController.State == BossStates.Intro)
            {
                self.transform.Find("Music").gameObject.GetComponent<AudioSource>().Stop();
                self.Boss.StopSinging();
            }
        }

    }
}