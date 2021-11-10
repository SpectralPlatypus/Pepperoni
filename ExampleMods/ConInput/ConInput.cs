using Pepperoni;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NativeCInput
{
    public class NativeCInput : Mod
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        private const string _modVersion = "1.0";
        NoidJSL Controller;
        IntPtr handle  = IntPtr.Zero;

        public NativeCInput() : base("NativeCInput")
        {
        }

        ~NativeCInput()
        {
            if(handle != IntPtr.Zero)
            {
                FreeLibrary(handle);
            }
        }

        public override string GetVersion() => _modVersion;

        public override void Initialize()
        {
            string dllPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "JoyShockLibrary");
            handle = LoadLibrary(Path.Combine(dllPath, "JoyShockLibrary.dll"));

            if(handle == IntPtr.Zero)
            {
                LogError("Failed to load JoyShock Library");
                return;
            }

            Controller = new NoidJSL();

            if(!Controller.Initialize())
            {
                LogError("Failed to initialize controller, falling back to default input manager");
                return;
            }

            if (!Kueido.InitStuff)
            {
                LogDebug("Kueido not initialized, delay attach");
                On.Kueido.FixedUpdate += Kueido_FixedUpdate;
            }
            else
            {
                Kueido.Controller.AssignController(Controller);
            }
        }

        private void Kueido_FixedUpdate(On.Kueido.orig_FixedUpdate orig, Kueido self)
        {
            orig(self);
            if(Kueido.InitStuff)
            {
                Kueido.Controller.AssignController(Controller);
                On.Kueido.FixedUpdate -= Kueido_FixedUpdate;
                LogDebug("Attached controller instance");
            }
        }
    }
}