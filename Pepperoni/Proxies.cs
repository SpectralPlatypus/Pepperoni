using UnityEngine;

namespace Pepperoni
{
    /// <summary>
    /// Allows modification of NPC text before Dialogue
    /// </summary>
    /// <param name="text">Dialogue Script</param>
    /// <returns>New or modified dialogue script</returns>
    public delegate string ParseScriptProxy(string text);

    /// <summary>
    /// Proxy for passing custom soundclips to speaker instance
    /// </summary>
    /// <param name="clip">Original soundclip </param>
    /// <returns>Original or new soundclip </returns>
    public delegate AudioClip SpeakerPlaySoundProxy(AudioClip clip);
}
