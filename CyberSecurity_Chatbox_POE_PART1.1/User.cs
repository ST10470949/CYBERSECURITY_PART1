using System;
using System.Collections.Generic;
using System.IO;

// Simple user model representing a named user.
class User
{
    // Name of the user
    public string Name { get; set; }

    // Construct a user with a name (safely handle null)
    public User(string name)
    {
        Name = name ?? string.Empty;
    }
}

// UserStore keeps lightweight persistent usage counts and current user state.
static class UserStore
{
    // Current user's name (shared across files)
    public static string CurrentUserName { get; set; } = string.Empty;

    // In-memory usage counts (case-insensitive keys)
    private static readonly Dictionary<string, int> counts = new(StringComparer.OrdinalIgnoreCase);

    // Default backing file for counts. Can be overridden if needed.
    public static string CountsFilePath { get; set; } = Path.Combine(AppContext.BaseDirectory, "user_counts.txt");

    // Load counts from disk into memory (non-fatal on error)
    public static void LoadCounts()
    {
        counts.Clear();
        try
        {
            if (!File.Exists(CountsFilePath)) return;
            foreach (var line in File.ReadAllLines(CountsFilePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var idx = line.LastIndexOf(':');
                if (idx <= 0) continue;
                var n = line.Substring(0, idx).Trim();
                var s = line.Substring(idx + 1).Trim();
                if (int.TryParse(s, out int v)) counts[n] = v;
            }
        }
        catch
        {
            // non-fatal, keep in-memory as empty
        }
    }

    // Save current in-memory counts to disk (best-effort)
    public static void SaveCounts()
    {
        try
        {
            var lines = new List<string>();
            foreach (var kv in counts)
            {
                lines.Add($"{kv.Key}:{kv.Value}");
            }
            File.WriteAllLines(CountsFilePath, lines);
        }
        catch
        {
            // ignore write failures
        }
    }

    // Increment usage count for a given user and persist
    public static int IncrementCount(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "(unknown)";
        counts.TryGetValue(name, out int prev);
        counts[name] = prev + 1;
        try { SaveCounts(); } catch { }
        return counts[name];
    }

    // Get stored count (0 if missing)
    public static int GetCount(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return 0;
        counts.TryGetValue(name, out int v);
        return v;
    }
}
