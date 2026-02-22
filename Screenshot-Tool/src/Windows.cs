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

        WindowInfo[] windows = [.. (JsonSerializer.Deserialize(processOutput, WindowJsonContext.Default.WindowInfoArray) ?? []).Where(w => !String.Equals(w.Title, "Screenshot-Tool"))];

        foreach (WindowInfo window in windows)
        {
            window.DisplayTitle = window.Class switch
            {
                "org.wezfurlong.wezterm" => GetWeztermWindowTitle(window.Title),
                "btop" => "Btop",
                "clipse" => "Clipse",
                _ => window.Title,
            };
        }

        return windows is not null
            ? windows
            : throw new InvalidOperationException("Failed to parse monitor information.");
    }

    private static string GetWeztermWindowTitle(string windowTitle)
    {
        string title = windowTitle;
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (title.StartsWith("Yazi: ", StringComparison.Ordinal))
        {
            title = title[6..];
        }
        if (title.StartsWith("~", StringComparison.Ordinal))
        {
            title = Path.Combine(homeDir, title[1..].TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        title = title.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        String folderName = Path.GetFileName(title);
        if (String.IsNullOrWhiteSpace(folderName))
        {
            folderName = "root";
        }

        return $"Wezterm - {folderName}";
    }
}