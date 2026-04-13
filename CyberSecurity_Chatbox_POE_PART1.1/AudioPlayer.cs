using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

// AudioPlayer provides simple audio playback helpers used by the app.
static class AudioPlayer
{
    // Synchronously play an audio file from the application base directory if it exists.
    // PlayIfExists synchronously plays the named audio file if it exists.
    // Any errors are swallowed to avoid crashing the app.
    public static void PlayIfExists(string filename)
    {
        try
        {
            string path = Path.Combine(AppContext.BaseDirectory, filename);
            if (File.Exists(path))
            {
                using SoundPlayer player = new SoundPlayer(path);
                player.Load();
                player.PlaySync();
            }
        }
        catch
        {
            // ignore audio errors to avoid crashing the app
        }
    }

    // Fire-and-forget asynchronous playback (non-blocking)
    // PlayIfExistsAsync starts playback on a background thread (non-blocking).
    public static void PlayIfExistsAsync(string filename)
    {
        _ = Task.Run(() => PlayIfExists(filename));
    }

    // Try to play but return success state
    // TryPlay attempts to play and returns true on success or false on failure.
    public static bool TryPlay(string filename)
    {
        try
        {
            string path = Path.Combine(AppContext.BaseDirectory, filename);
            if (!File.Exists(path)) return false;
            using SoundPlayer player = new SoundPlayer(path);
            player.Load();
            player.PlaySync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
