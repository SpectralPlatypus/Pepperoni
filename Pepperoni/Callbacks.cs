using UnityEngine;

namespace Pepperoni
{
    /// <summary>
    /// Called after a collectible object collides with another world object
    /// </summary>
    /// <param name="count">Current number of collectibles</param>
    /// <param name="other">Other object. Collectibles are only activated if this object is Player</param>
    public delegate void CollectibleCallback(int count, Collider other);

    /// <summary>
    /// Called before a scene is loaded
    /// </summary>
    /// <param name="sceneName">New Scene Name</param>
    public delegate void BeforeSceneLoadCallback(string sceneName);

    /// <summary>
    /// Called once a scene has been loaded
    /// </summary>
    /// <param name="sceneName">New Scene Name</param>
    public delegate void AfterSceneLoadCallback(string sceneName);

    /// <summary>
    /// Called when the Player leaves the loaded stage (level is fully loaded)
    /// </summary>
    public delegate void PlayerUnLoadedCallback();

    /// <summary>
    /// Called when the game sets player character
    /// This is the best spot for loading custom textures, normal maps, renderers etc
    /// </summary>
    /// <param name="skinnedMeshRenderer">Skinned Mesh Renderer used for the current character</param>
    public delegate void PlayerSetCostumeCallback(SkinnedMeshRenderer skinnedMeshRenderer);

    /// <summary>
    /// Called when PlayerMachine completes early update call
    /// This is usually where character specific mechanics should be updated
    /// </summary>
    public delegate void PlayerEarlyUpdateCallback(PlayerMachine playerMachine);

    //public delegate void NewGameHandler();

    /// <summary>
    /// Called before the game attempts to save data
    /// </summary>
    public delegate void SaveGameBeforeSaveCallback();

    /// <summary>
    /// Called after the game finishes saving data.
    /// Ideal for point for mods to save their internal data
    /// </summary>
    public delegate void SaveGameAfterSaveCallback();

    /// <summary>
    /// Called after the game finishes saving data.
    /// Ideal for point for mods to save their internal data
    /// </summary>
    public delegate void NewGameStartedCallback();
}
