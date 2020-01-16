namespace Pepperoni
{
    public interface IMod : ILogger
    {
        /// <summary>
        /// Getter for the Mod Name
        /// </summary>
        /// <returns>Mod name</returns>
        string GetName();

        /// <summary>
        /// Called when the Modloader installs or activates the mod
        /// </summary>
        void Initialize();

        /// <summary>
        /// Getter for the mod version
        /// </summary>
        /// <returns>Mod Version</returns>
        string GetVersion();

        /// <summary>
        /// Returns the mod priority
        /// </summary>
        /// <returns>Mods are loaded according to their priority in ascending order</returns>
        int LoadPriority();
    }
}
