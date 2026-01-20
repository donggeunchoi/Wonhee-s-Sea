using UnityEngine;
using System;

public static class DataParse
{
    public static T ParseSafe<T>(string s, T defaultValue = default)
    {
        if (string.IsNullOrEmpty(s)) return defaultValue;

        if (typeof(T) == typeof(int)) return (T)(object)(int.TryParse(s, out var v) ? v : (object)defaultValue);
        if (typeof(T) == typeof(float)) return (T)(object)(float.TryParse(s, out var v) ? v : (object)defaultValue);
        if (typeof(T) == typeof(bool))
        {
            var lower = s.ToLower();
            return (T)(object)(lower == "true" || lower == "1" || lower == "y");
        }
        if (typeof(T) == typeof(string)) return (T)(object)s;

        return defaultValue;
    }

    public static T EnumParseSafe<T>(string raw, T @default) where T : struct, Enum =>
        string.IsNullOrWhiteSpace(raw) ? @default : (Enum.TryParse<T>(raw.Trim(), true, out var val) ? val : @default);
}
