using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Pepperoni
{
    public static class DialogueUtils
    {
        public const string PassageEnd = "\r\n\r\n";
        public const string DialogEnd = "\r\n\r\n%n";
        public const string Pause = "%m0%%s1%%m1%%sD%";
        public const string PauseBrief = "%m0%%s.5%%m1%%sD%";

        public static string EmitNewPassageHeader(DialogPortraits dialogPortrait, DialogVoices dialogVoice, string npcName = "Noid", bool fade = true)
        {
            StringBuilder dialog = new StringBuilder(32);
            dialog.Append(fade ? "%n" : "%r");
            dialog.Append((int)dialogPortrait);
            dialog.AppendLine($"%v{(int)dialogVoice}%");
            dialog.AppendLine(npcName);
            return dialog.ToString();
        }

        public static string MouthMove(uint isMouthMoving) => string.Concat("%m",isMouthMoving,"%");

        public static string PauseDelay(float pauseDelay) => string.Concat("%s", pauseDelay.ToString(), "%");

        public static string PauseDelayDefault() => "%sD%";

        public static string SoundFX(uint soundFxIndex) 
            => "%e" + ((soundFxIndex < 5) ? soundFxIndex.ToString() : "5") + "%";

        public static string GetNPCName(string dialogue)
        {
            string pattern = @"%v\d+%\s*(\w+)";
            Match m = Regex.Match(dialogue, pattern);
            while(m.Success && m.Groups[1].Value == "Noid")
            {
                m = m.NextMatch();
            }

            return m.Success ? m.Groups[1].Value : string.Empty;
        }
    }
    public enum DialogPortraits
    {
        NoidNeutral = 0,
        Noid4thWall = 1,
        NoidSurprise = 2,
        NoidAngry = 3,
        NoidDab = 7,
        Tomato = 8,
        Mushroom = 9,
        OliveGreen = 10,
        GarlicDip = 11,
        MikeCalm = 12,
        MikeUpset = 13,
        MikeDying = 14,
        OliveBlack = 15,
        ChickenDinner = 16,
        Pineapple = 17
    }

    public enum DialogVoices
    {
        OliveMale = 0,
        Mushroom = 1,
        Tomato = 2,
        GarlicDip = 4,
        OliveFemale = 6,
        NoidNeutral = 8,
        Noid4thWall = 9,
        NoidSurprise = 10,
        NoidAngry = 11,
        ChickenDinner = 12,
        Pineapple = 13
    }
}
