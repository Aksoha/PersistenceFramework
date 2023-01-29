﻿using System.Reflection;

namespace BlazorExample;

public static class ApplicationInfo
{
    public static readonly string ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

    public static readonly string SettingsFile = Path.Combine(ExecutingDirectory, "settings.json");
}