using Pepperoni;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PPR_Standalone
{
    public class PPRMod : Mod
    {
        public PPRMod() : base("PPR", "2.1")
        {
        }

        private const float zapPercent = 0.07f;
        private readonly Vector3 npcPos = new Vector3(777.6f, 306.6f, 436.8f);
        private readonly Quaternion npcRot = new Quaternion(-0.2f, -0.2f, 0.1f, 0.9f);
        private readonly Vector3 camPos = new Vector3(776.5f, 302.1f, 460.3f);
        private bool cheeseAttempt = false;
        private bool cheeseDialog = true;
        private FieldInfo levelStr = typeof(PizzaBox).GetField("LevelToLoad", BindingFlags.Instance | BindingFlags.NonPublic);
        private Vector3 defaultWarp = new Vector3(854.5f, 58.6f, 223.4f);

        public override void Initialize()
        {
            SceneManager.activeSceneChanged += OnSceneChange;
            ModHooks.Instance.GetCollectibleHook += OnPepObtain;
            ModHooks.Instance.OnParseScriptHook += OnParseScript;
            On.PlanetDoor.Tugged += PlanetDoor_Tugged;
            On.FadeoutLoad.OnTriggerEnter += FadeoutLoad_OnTriggerEnter;

            // All this effort to prevent Crust from cheesing the door mechanics
            On.CameraLocation.OnTriggerEnter += CameraLocation_OnTriggerEnter;
            On.CameraLocation.OnTriggerExit += CameraLocation_OnTriggerExit;
        }

        private void CameraLocation_OnTriggerExit(On.CameraLocation.orig_OnTriggerExit orig, CameraLocation self, Collider Col)
        {
            if (SceneManager.GetActiveScene().name == "void" && !gotAllPep()
                && Vector3.Distance(self.transform.position, camPos) < 10f)
            {
                return;
            }
            orig(self, Col);
        }

        private void CameraLocation_OnTriggerEnter(On.CameraLocation.orig_OnTriggerEnter orig, CameraLocation self, Collider Col)
        {
            if (SceneManager.GetActiveScene().name == "void" && !gotAllPep()
                && Vector3.Distance(self.transform.position, camPos) < 10f)
            {
                cheeseAttempt = true;
                Manager.Player.GetComponent<PlayerMachine>().transform.position = npcPos;
            }
            else
            {
                orig(self, Col);
            }
        }

        private bool gotAllPep() => (SaveScript.doesLevelHaveAllCollectibles("LeviLevle")
                     && SaveScript.doesLevelHaveAllCollectibles("dungeon")
                     && SaveScript.doesLevelHaveAllCollectibles("PZNTv5"));

        private void FadeoutLoad_OnTriggerEnter(On.FadeoutLoad.orig_OnTriggerEnter orig, FadeoutLoad self, Collider other)
        {
            // Sanity measure
            if (other.gameObject.tag == "Player" && gotAllPep())
            {
                orig(self, other);
            }
        }

        private void PlanetDoor_Tugged(On.PlanetDoor.orig_Tugged orig, PlanetDoor self)
        {
            // Basic measure for all except Crust...
            if (!gotAllPep())
            {
                return;
            }

            orig(self);
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
                    Manager.Dialogue.UpdateCostumePortrait();

                    if (UnityEngine.Random.value <= zapPercent)
                    {
                        LogDebug($"{_name}: Zaptime!");
                        p.GetStunned(1.2f);
                    }
                }
            }
        }
        private string[] costumeNames = { "Noid", "Jumpy", "Hedgehog", "Stubby" };
        private static Dictionary<String, Vector3> levelPos = new Dictionary<string, Vector3>();
        public string OnParseScript(string text)
        {
            if (text.Contains("Oleia"))
            {
                var playerPos = Manager.Player.GetComponent<PlayerMachine>().transform.position;
                if (SceneManager.GetActiveScene().name != "void" || Vector3.Distance(npcPos, playerPos) > 10f)
                    return text;

                if (Kueido.Input.Dab.Held)
                {
                    Vector3 warpPos = defaultWarp;
                    foreach (var level in levelPos.Keys)
                    {
                        if (!SaveScript.doesLevelHaveAllCollectibles(level))
                        {
                            warpPos = levelPos[level];
                            break;
                        }
                    }
                    Manager.Player.GetComponent<PlayerMachine>().transform.position = warpPos;
                    return "%n10%v6%\r\nLegs\r\n%m1%Looks like you have not completed this level yet.\r\n\r\n%n";
                }

                StringBuilder sb = new StringBuilder();
                sb.Append($"%n10%v6%\r\nLegs\r\n%m1%Hey there {costumeNames[(int)PlayerMachine.CurrentCostume]}! ");

                if (gotAllPep())
                {
                    sb.Append("%m0%%s1%%m1%%sD%I can feel the power of Pepperoni.%m0%%s1%%m1%%sD% Our time draws near.\r\n\r\n" +
                    "%n1%v9%\r\nNoid\r\n%m1%Sounds like you have been on this planet for too long.\r\n\r\n" +
                    "%n10%v6%\r\nLegs\r\n%m1%GO FORTH WARRIOR!%m0%%s.1%%m1% VANQUISH THE EVIL THAT LIES AHEAD!\r\n\r\n" +
                    "%n2%v10%\r\nNoid\r\n%m1%Okay...\r\n\r\n%n");

                }
                else if (cheeseAttempt && cheeseDialog)
                {
                    cheeseDialog = false;
                    return "%n10%v6%\r\nLegs\r\n%m1%DID YOU SERIOUSLY THINK THAT WAS GOING TO WORK???\r\n\r\n%n";
                }
                else
                {
                    sb.Append("%m0%%s1%%m1%%sD%Looks like you can't enter the castle just yet.\r\n\r\n%n0%v8%\r\nNoid\r\n%m1" +
                    "%What gives?\r\n\r\n %n10%v6%\r\nLegs\r\n%m1%Only those wielding the strongest passion can enter..." +
                    "%m0%%s.1%%m1% PEPPERONI PASSION!\r\n\r\n%n2%v10%\r\nNoid\r\n%m1%Pepperoni Passion?!?\r\n\r\n" +
                    "%n10%v6%\r\nLegs\r\n%m1%In other words, you have to collect every single Pepperoni in all 3 stages.\r\n\r\n" +
                    "%n3%v11%\r\nNoid\r\n%m1%How am I supposed to do that? I am stranded here now.\r\n\r\n" +
                    "%n10%v6%\r\nLegs\r\n%m1%Oh, about that... I can send you down if you need to head back.\r\n\r\n" +
                    "%n2%v10%\r\nNoid\r\n%m1%How are you going to do that?\r\n\r\n" +
                    "%n10%v6%\r\nLegs\r\n%m1%Nevermind that. Give me a sign next time you talk to me! (Hold Right click/Left bumper)\r\n\r\n%n");
                }
                return sb.ToString();
            }

            return text;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            if (newScene.name == "void")
            {
                Basic_NPC legsNpc = null;
                var npcs = Object.FindObjectsOfType<Basic_NPC>();

                foreach (var npc in npcs)
                {
                    var textAsset = npc.GetComponentInChildren<TalkVolume>().Dialogue;
                    if (DialogueUtils.GetNPCName(textAsset.text) == "Oleia")
                    {
                        legsNpc = npc;
                        break;
                    }
                }
                if (legsNpc != null)
                {
                    LogDebug("Found Oleia (Legs)!");
                    Object.Instantiate(legsNpc, npcPos, npcRot);
                }

                levelPos.Clear();
                var boxes = GameObject.FindObjectsOfType<PizzaBox>();
                foreach (var box in boxes)
                {
                    string level = levelStr.GetValue(box) as string;
                    if (level != null)
                    {
                        levelPos[level] = box.gameObject.transform.position;
                    }
                }
            }
        }
    }
}
