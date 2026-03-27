using System.Diagnostics;
using System.Text.Json;

namespace Screenshot_Tool.src.Types;

public static class HyprlandProvider
{
  public static Monitor[] GetAvailableMonitors()
  {
    using Process process = ProcessHelper.CreateProcess("hyprctl", redirectStdIn: false, redirectStdOut: true, ["monitors", "-j"]);
    process.Start();

    String processOutput = process.StandardOutput.ReadToEnd();

    process.WaitForExit();

    Monitor[] monitors = JsonSerializer.Deserialize(processOutput, MonitorJsonContext.Default.MonitorArray) ?? [];

    return monitors.Length > 0
      ? monitors : throw new InvalidOperationException("Failed to parse monitor information.");
  }

  public static Window[] GetAvailableWindows()
  {
    using Process process = ProcessHelper.CreateProcess("hyprctl", redirectStdIn: false, redirectStdOut: true, ["clients", "-j"]);
    process.Start();

    String processOutput = process.StandardOutput.ReadToEnd();

    process.WaitForExit();

    Int32 thisProcessID = Environment.ProcessId;
    Int32 parentProcessID = 0;

    using Process currentProcess = Process.GetCurrentProcess();
    parentProcessID = GetParentPid(thisProcessID);

    Window[] windows = [.. (JsonSerializer.Deserialize(processOutput, WindowJsonContext.Default.WindowArray) ?? [])
      .Where(w => w.ProcessID != thisProcessID && w.ProcessID != parentProcessID && !String.IsNullOrWhiteSpace(w.Title))];
    windows = UpdateWindowDisplayTitles(windows);

    return windows.Length > 0
      ? windows : throw new InvalidOperationException("Failed to parse window information.");
  }

  private static Window[] UpdateWindowDisplayTitles(Window[] windows)
  {
    Dictionary<String, String> classMappings = LoadClassMappings();

    foreach (Window window in windows)
    {
      String lowerClass = window.Class.ToLower().Trim();
      KeyValuePair<String, String> match = classMappings.FirstOrDefault(m => lowerClass.Contains(m.Key.ToLower()));

      window.DisplayTitle = match.Value is not null
        ? match.Value : GetTerminalDisplayTitle(window.Title);
    }
    return windows;
  }

  private static String GetTerminalDisplayTitle(String windowTitle)
  {
    String title = windowTitle;
    if (title.StartsWith("Yazi: ", StringComparison.Ordinal))
    {
      title = title[6..];
    }

    if (title.StartsWith('~') || title.StartsWith('/'))
    {
      title = title.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
      String? folderName = Path.GetFileName(title);

      if (String.IsNullOrWhiteSpace(folderName) || folderName == "~")
      {
        folderName = "Home";
      }
      else if (folderName == "/")
      {
        folderName = "Root";
      }

      title = $"Shell - {folderName}";
    }

    return title;
  }

  private static Int32 GetParentPid(Int32 pid)
  {
    try
    {
      String stat = File.ReadAllText($"/proc/{pid}/stat");
      return Int32.Parse(stat.Split(' ')[3]);
    }
    catch { return 0; }
  }

  private static Dictionary<String, String> LoadClassMappings()
  {
    Dictionary<String, String> classMappings = new(StringComparer.OrdinalIgnoreCase);
    String configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "screenshot-tool");
    String configPath = Path.Combine(configDirectory, "class_mappings.conf");

    if (!Directory.Exists(configDirectory))
    {
      Directory.CreateDirectory(configDirectory);
    }

    if (!File.Exists(configPath))
    {
      File.WriteAllText(configPath, String.Empty);
    }

    foreach (String line in File.ReadAllLines(configPath))
    {
      if (String.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
      {
        continue;
      }

      String[] parts = line.Split('=', 2);
      if (parts.Length == 2)
      {
        classMappings[parts[0].Trim()] = parts[1].Trim();
      }
    }
    return classMappings;
  }
}
