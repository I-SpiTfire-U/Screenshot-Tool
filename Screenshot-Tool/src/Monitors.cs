using System.Diagnostics;
using System.Text.Json;

namespace Screenshot_Tool.src;

public static class Monitors
{
    public static MonitorInfo[] GetAvailableMonitors()
    {
        string[] arguments = ["monitors", "-j"];
        using Process process = ProcessHelper.CreateProcess("hyprctl", redirectStdIn: false, redirectStdOut: true, args: arguments);

        process.Start();

        string processOutput = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        MonitorInfo[]? monitors = JsonSerializer.Deserialize(processOutput, ScreenshotJsonContext.Default.MonitorInfoArray);

        return monitors is not null
            ? monitors
            : throw new InvalidOperationException("Failed to parse monitor information.");
    }
}