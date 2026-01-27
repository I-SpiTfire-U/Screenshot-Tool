using System.Diagnostics;
using System.Text.Json;

namespace Screenshot_Tool.src;

public static class Windows
{
    public static WindowInfo[] GetAvailableWindows()
    {
        string[] arguments = ["clients", "-j"];
        using Process process = ProcessHelper.CreateProcess("hyprctl", redirectStdIn: false, redirectStdOut: true, args: arguments);

        process.Start();

        string processOutput = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        WindowInfo[]? windows = JsonSerializer.Deserialize(processOutput, WindowJsonContext.Default.WindowInfoArray);

        return windows is not null
            ? windows
            : throw new InvalidOperationException("Failed to parse monitor information.");
    }
}