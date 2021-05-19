using Pepperoni;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BuilderNoid
{
    public class BuilderNoid : Pepperoni.Mod
    {

        internal static Texture2D blockTexture = null;
        private GameObject[] blockList = new GameObject[5];
        private int _blockIndex = 0;
        private static readonly int BLAYER = 15;
        GameObject protoBlock;

        public BuilderNoid() : base("BuilderNoid")
        {
        }

        public override string GetVersion() => "2.0";

        public override void Initialize()
        {
            string fileName = "";
            foreach (string fn in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (fn.Contains("pizza_box"))
                {
                    fileName = fn;
                    break;
                }
            }

            try
            {
                if (fileName == "")
                    throw new FileNotFoundException();

                using (Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
                {
                    byte[] imageBuffer = new byte[imageStream.Length];
                    imageStream.Read(imageBuffer, 0, imageBuffer.Length);
                    imageStream.Flush();
                    blockTexture = new Texture2D(1, 1);
                    blockTexture.LoadImage(imageBuffer);
                    LogDebug("Loaded Block Texture");
                    ModHooks.Instance.OnPlayerEarlyUpdateHook += OnEarlyUpdate;
                    ModHooks.Instance.BeforeSceneLoad += Instance_BeforeSceneLoad;

                    protoBlock = new GameObject();
                    protoBlock.AddComponent<ProtoBlock>();
                    GameObject.DontDestroyOnLoad(protoBlock);
                    protoBlock.SetActive(false);
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        private void Instance_BeforeSceneLoad(string sceneName)
        {
            for (int i = 0; i < blockList.Length; ++i)
            {
                GameObject.Destroy(blockList[i]);
                blockList[i] = null;
            }
            _blockIndex = 0;
        }

        enum BlockState
        {
            Idle,
            Placing,
            Over
        }

        BlockState state = BlockState.Idle;
        private void OnEarlyUpdate(PlayerMachine playerMachine)
        {
            if (state != BlockState.Idle && playerMachine.currentState.Equals(PlayerStates.Loading))
            {
                protoBlock.SetActive(false);
                state = BlockState.Idle;
            }

            switch (state)
            {
                case BlockState.Idle:
                    if (Kueido.Input.Dab.Pressed)
                    {
                        protoBlock.SetActive(true);

                        state = BlockState.Placing;
                    }
                    break;
                case BlockState.Placing:
                    if (Kueido.Input.Dab.Held)
                    {
                        Vector3 normal =
                            (playerMachine.currentState.Equals(PlayerStates.Walk)) ? playerMachine.controller.currentGround.PrimaryNormal() : playerMachine.controller.up;
                        protoBlock.transform.position = playerMachine.transform.position +
                            (normal * 2f) +
                            (playerMachine.lookDirection * 5 * (playerMachine.Camera.zoom2 + 0.3f));
                        protoBlock.transform.up = normal;
                    }
                    else
                    {
                        // Place block etc
                        if (protoBlock.GetComponent<ProtoBlock>().IsFree())
                        {
                            PlaceBlock(protoBlock.transform.position, protoBlock.transform.rotation);
                        }
                        protoBlock.SetActive(false);
                        state = BlockState.Idle;
                    }
                    break;
            }
        }

        private void PlaceBlock(Vector3 blockPos, Quaternion rotation)
        {
            int index = _blockIndex % blockList.Length;
            if (blockList[index] == default(GameObject))
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);

                go.layer = BLAYER;
                go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                go.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", blockTexture);
                go.SetActive(true);

                blockList[index] = go;

            }

            blockList[index].transform.position = blockPos;
            blockList[index].transform.rotation = rotation;
            _blockIndex = ++index % blockList.Length;
        }
    }
}