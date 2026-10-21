using System.Collections.Generic;
using UnityEngine;

public static class AudioPreferences
{
    private enum Channel { Music, Sfx }
    private sealed class Entry
    {
        public AudioSource Source;
        public float BaseVolume;
        public Channel Channel;
    }

    private static readonly List<Entry> Sources = new List<Entry>();
    private static bool loaded;
    public static float Master { get; private set; } = 1f;
    public static float Music { get; private set; } = 0.8f;
    public static float Sfx { get; private set; } = 1f;

    public static void RegisterMusic(AudioSource source) => Register(source, Channel.Music);
    public static void RegisterSfx(AudioSource source) => Register(source, Channel.Sfx);

    private static void Register(AudioSource source, Channel channel)
    {
        if (source == null) return;
        Load();
        Entry entry = Sources.Find(item => item.Source == source);
        if (entry == null)
        {
            entry = new Entry { Source = source, BaseVolume = source.volume, Channel = channel };
            Sources.Add(entry);
        }
        else entry.Channel = channel;
        Apply(entry);
    }

    public static void SetMaster(float value) { Load(); Master = Mathf.Clamp01(value); SaveAndApply(); }
    public static void SetMusic(float value) { Load(); Music = Mathf.Clamp01(value); SaveAndApply(); }
    public static void SetSfx(float value) { Load(); Sfx = Mathf.Clamp01(value); SaveAndApply(); }

    public static void Load()
    {
        if (loaded) return;
        loaded = true;
        Master = PlayerPrefs.GetFloat("audio.master", 1f);
        Music = PlayerPrefs.GetFloat("audio.music", 0.8f);
        Sfx = PlayerPrefs.GetFloat("audio.sfx", 1f);
    }

    private static void SaveAndApply()
    {
        PlayerPrefs.SetFloat("audio.master", Master);
        PlayerPrefs.SetFloat("audio.music", Music);
        PlayerPrefs.SetFloat("audio.sfx", Sfx);
        PlayerPrefs.Save();
        for (int i = Sources.Count - 1; i >= 0; i--)
        {
            if (Sources[i].Source == null) Sources.RemoveAt(i);
            else Apply(Sources[i]);
        }
    }

    private static void Apply(Entry entry)
    {
        float channelVolume = entry.Channel == Channel.Music ? Music : Sfx;
        entry.Source.volume = entry.BaseVolume * channelVolume * Master;
    }
}
