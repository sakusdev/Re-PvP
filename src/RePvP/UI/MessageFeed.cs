using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RePvP;

public sealed class MessageFeed
{
    private sealed class Entry
    {
        public Entry(string text, float expiresAt)
        {
            Text = text;
            ExpiresAt = expiresAt;
        }

        public string Text { get; }
        public float ExpiresAt { get; }
    }

    private readonly List<Entry> _entries = new();

    public void Push(string text, float durationSeconds = 4f)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        _entries.Add(new Entry(text, Time.unscaledTime + durationSeconds));
        Plugin.Log.LogInfo($"HUD message: {text}");
    }

    public IReadOnlyList<string> GetVisibleMessages()
    {
        var now = Time.unscaledTime;
        _entries.RemoveAll(entry => entry.ExpiresAt <= now);
        return _entries.Select(entry => entry.Text).ToList();
    }

    public void Clear()
    {
        _entries.Clear();
    }
}
