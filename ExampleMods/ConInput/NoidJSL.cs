using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Pepperoni;

namespace NativeCInput
{
    class NoidJSL : ControllerManagement.ControllerStub
    {
        int ctrlHandle = 0;

        ~NoidJSL()
        {
            JSL.JslDisconnectAndDisposeAll();
        }

        public bool Initialize()
        {
            int numDev = JSL.JslConnectDevices();
            if (numDev == 0)
            {
                Logger.LogError("Failed to find devices, using the default controller");
                return false;
            }

            // We will only use the first controller
            // TODO: Swag for DS4
            int[] devHandles = new int[numDev];
            int retVal = JSL.JslGetConnectedDeviceHandles(devHandles, numDev);
            Logger.LogDebug($"Got {retVal} Devices");
            if (retVal != numDev)
            {
                Logger.LogError("Mismatching number of handles");
                return false;
            }

            ctrlHandle = devHandles[0];
            int color = 0xff00ff;

            int ctrlType = JSL.JslGetControllerType(ctrlHandle);
            if (ctrlType == JSL.DUAL_SHOCK4 || ctrlType == JSL.DUAL_SENSE)
            {
                Logger.Log("Found DS4, Setting color");
                JSL.JslSetLightColour(ctrlHandle, color);
                ModHooks.Instance.OnPlayerSetCostumeHook += Instance_OnPlayerSetCostumeHook;
            }
            else if (ctrlType == JSL.SWITCH_PRO)
            {
                Logger.Log("Found Switch Pro Controller");
            }

            return true;
        }

        public override float GetAnalogState(Analog a)
        {
            switch(a)
            {
                case Analog.LeftStickX:
                    return JSL.JslGetLeftX(ctrlHandle);
                case Analog.LeftStickY:
                    return -1f * JSL.JslGetLeftY(ctrlHandle);
                case Analog.RightStickX:
                    return JSL.JslGetRightX(ctrlHandle);
                case Analog.RightStickY:
                    return -1f * JSL.JslGetRightY(ctrlHandle);
                case Analog.TriggerLeft:
                    return JSL.JslGetLeftTrigger(ctrlHandle);
                case Analog.TriggerRight:
                    return JSL.JslGetRightTrigger(ctrlHandle);
            }

            return 0f;
        }

        public override bool GetButtonPressed(Button b)
        {
            /*
             * NOT USED
            */
            return false;
        }

        public override bool GetButtonReleased(Button b)
        {
            /*
             * NOT USED
            */
            return false;
        }


        public override bool GetButtonState(Button b)
        {
            var state = JSL.JslGetSimpleState(ctrlHandle);
            int mask = 1 << buttonBitMap[b];
            return (state.buttons & mask) == mask;
        }

        public override void Init()
        {
        }

        private void Instance_OnPlayerSetCostumeHook(UnityEngine.SkinnedMeshRenderer skinnedMeshRenderer)
        {
            int color = 0;
            switch (PlayerMachine.CurrentCostume)
            {
                case Costumes.Default:
                    color = 0xFF0000;
                    break;
                case Costumes.Green:
                    color = 0x00FF00;
                    break;
                case Costumes.Fast:
                    color = 0x0000FF;
                    break;
                case Costumes.Capsule:
                    color = 0xFFA500;
                    break;
                case Costumes.Miku:
                    color = 0x137A7F;
                    break;
            }

            JSL.JslSetLightColour(ctrlHandle, color);
        }

        public override void Update()
        {
        }
        
        readonly Dictionary<Button, int> buttonBitMap = new Dictionary<Button, int>
        {
            {Button.Right, JSL.ButtonMaskRight },
            {Button.Left, JSL.ButtonMaskLeft },
            {Button.Up, JSL.ButtonMaskUp },
            {Button.Down, JSL.ButtonMaskDown },
            {Button.A, JSL.ButtonMaskS },
            {Button.B, JSL.ButtonMaskE },
            {Button.X, JSL.ButtonMaskW },
            {Button.Y, JSL.ButtonMaskN },
            {Button.Start, JSL.ButtonMaskOptions },
            {Button.Select, JSL.ButtonMaskShare },
            {Button.LeftThumb, JSL.ButtonMaskLClick },
            {Button.RightThumb, JSL.ButtonMaskRClick },
            {Button.LeftBumper, JSL.ButtonMaskL },
            {Button.RightBumper, JSL.ButtonMaskR },
            {Button.LeftTrigger, JSL.ButtonMaskZL },
            {Button.RightTrigger, JSL.ButtonMaskZR }
        };
    }
}
