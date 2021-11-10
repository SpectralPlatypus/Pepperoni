using MonoMod;
using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0626, CS0108
namespace Pepperoni.Patches
{
    [MonoModPatch("global::ControllerManagement")]
    class ControllerManagement : global::ControllerManagement
    {
        [MonoModIgnore] private ControllerStub CurrentController;
        //bool requestCtrlChange = false;
        //ControllerStub tmpStub = null;

        [MonoModPublic]
        public abstract class ControllerStub { }

        public void AssignController(ControllerStub newController)
        {
            CurrentController = newController;
        }

        /*
        public extern void orig_ManagedEarlyUpdate();
        public void ManagedEarlyUpdate()
        {
            orig_ManagedEarlyUpdate();
            if (requestCtrlChange)
            {
                ModHooks.Instance.OnNewGameStart();
                requestCtrlChange = false;
            }
        }
        */
    }

    /*
    struct ButtonInput
    {
        public Button Type;
        public string Handle;
        public bool IsAnalog;
        public bool Invert;
        public float GateLevel;

    }
    
    struct AnalogInput
    {
        public Analog Type;
        public string Handle;
        public bool Invert;
    }

    abstract class ControllerProfile
    {

        public abstract bool GetButton(Button button);
        public abstract float GetAnalog(Analog button);
        public abstract string GetName();
    }

    class PS4InputProfile : ControllerProfile
    {
        static readonly string name = "Wireless Controller";

        ButtonInput[] ButtonMap = new[] {
            new ButtonInput { // Right
                Type = Button.Right,
                Handle = "X-Axis",
                IsAnalog = true,
                GateLevel = -0.5f
            },
            new ButtonInput { // Left
                Type = Button.Left,
                Handle = "X-Axis",
                IsAnalog = true,
                GateLevel = 0.5f
            },
            new ButtonInput { // Up
                Type = Button.Up,
                Handle = "Y-Axis",
                IsAnalog = true,
                Invert = true,
                GateLevel = 0.5f
            },
            new ButtonInput { // Down
                Type = Button.Down,
                Handle = "Y-Axis",
                IsAnalog = true,
                Invert = true,
                GateLevel = -0.5f
            },
            new ButtonInput { // Cross
                Type = Button.A,
                Handle = "joystick button 1",
                IsAnalog = false
            },
            new ButtonInput { // Circle
                Type = Button.B,
                Handle = "joystick button 2",
                IsAnalog = false
            },
            new ButtonInput { // Square
                Type = Button.X,
                Handle = "joystick button 0",
                IsAnalog = false
            },
            new ButtonInput { // Square
                Type = Button.Y,
                Handle = "joystick button 3",
                IsAnalog = false
            },
            new ButtonInput { // Start
                Type = Button.Start,
                Handle = "joystick button 9",
                IsAnalog = false
            },
            new ButtonInput { // Select
                Type = Button.Select,
                Handle = "joystick button 8",
                IsAnalog = false
            },
            new ButtonInput { // L3
                Type = Button.LeftThumb,
                Handle = "joystick button 10",
                IsAnalog = false
            },
            new ButtonInput { // R3
                Type = Button.RightThumb,
                Handle = "joystick button 11",
                IsAnalog = false
            },
            new ButtonInput { // L1
                Type = Button.LeftBumper,
                Handle = "joystick button 4",
                IsAnalog = false
            },
            new ButtonInput { // R1
                Type = Button.RightBumper,
                Handle = "joystick button 5",
                IsAnalog = false
            },
            new ButtonInput { // LT
                Type = Button.LeftTrigger,
                Handle = "5th Axis",
                IsAnalog = true,
                Invert = false,
                GateLevel = 0.95f

            },
            new ButtonInput { // RT
                Type = Button.RightTrigger,
                Handle = "6th Axis",
                IsAnalog = true,
                Invert = false,
                GateLevel = 0.95f
            },
        };

        AnalogInput[] AnalogMap = new[] {
            new AnalogInput { // Left X
                Type = Analog.LeftStickX,
                Handle = "X-Axis",
            },
            new AnalogInput { // Left Y 
                Type = Analog.LeftStickY,
                Handle = "Y-Axis",
                Invert = true,
            },
            new AnalogInput { // Right X
                Type = Analog.RightStickX,
                Handle = "3rd Axis",
            },
            new AnalogInput { // Down
                Type = Analog.RightStickY,
                Handle = "4th Axis",
                Invert = true,
            },
            new AnalogInput { // LT
                Type = Analog.TriggerLeft,
                Handle = "5th Axis",
            },
            new AnalogInput { // RT
                Type = Analog.TriggerRight,
                Handle = "6th Axis",
            }
        };

        public override float GetAnalog(Analog input)
        {
            return Input.GetAxisRaw(AnalogMap[(int)input].Handle);
        }

        public override bool GetButton(Button button)
        {
            var entry = ButtonMap[(int)button];
            if (!entry.IsAnalog)
            {
                return Input.GetButton(entry.Handle);
            }

            float value = Input.GetAxisRaw(entry.Handle);
            if (entry.Invert) value = -value;

            if(entry.GateLevel < 0)
                return value < entry.GateLevel;
            else
                return value > entry.GateLevel;
        }

        public override string GetName()
        {
            return name;
        }
    }*/
}