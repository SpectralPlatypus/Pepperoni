using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pepperoni
{
    ///<inheritdoc />
    /// <summary>
    /// Interface which signifies that this mod can be loaded _and_ unloaded while in game.  
    /// Implementing this interface requires that properly handle tracking every hook you add, game state that you change, so that you can disable it all.
    /// </summary>
    public interface IToggleMod : IMod
    {
        /// <summary>
        /// Called when the mod is disabled or unloaded
        /// </summary>
         void Unload();
    }
}
