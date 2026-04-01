using System.Diagnostics;
using Screenshot_Tool.src.Enums;
using Screenshot_Tool.src.Types;
using Monitor = Screenshot_Tool.src.Types.Monitor;

namespace Screenshot_Tool.src;

public static class ScreenShotting
{
  public static void TakeScreenshot(String rootDirectory, ScreenshotType screenshotType)
  {
    String grimGeometry;
    String screenshotPath;

    switch (screenshotType)
    {
      case ScreenshotType.AreaSelect:
        ProcessHelper.RunHyprctlProcess("movetoworkspacesilent", $"special:1,title:Screenshot-Tool");
        (grimGeometry, screenshotPath) = (GetSlurpGeometry(), ConstructScreenshotPath(rootDirectory, String.Empty));
        break;

      case ScreenshotType.SingleWindow:
        if (!TryGetSelectionFromAvailable(HyprlandProvider.GetAvailableWindows(), w => w.DisplayTitle, out Window? window) || window is null)
        {
          return;
        }
        ProcessHelper.RunHyprctlProcess("movetoworkspacesilent", $"special:1,title:Screenshot-Tool");
        (grimGeometry, screenshotPath) = (window.GetGrimGeometry(), ConstructScreenshotPath(rootDirectory, window.DisplayTitle));
        ProcessHelper.RunHyprctlProcess("focuswindow", $"class:{window.Class}");
        break;

      case ScreenshotType.SingleMonitor:
        if (!TryGetSelectionFromAvailable(HyprlandProvider.GetAvailableMonitors(), m => m.Name, out Monitor? monitor) || monitor is null)
        {
          return;
        }
        ProcessHelper.RunHyprctlProcess("movetoworkspacesilent", $"special:1,title:Screenshot-Tool");
        (grimGeometry, screenshotPath) = (monitor.GetGrimGeometry(), ConstructScreenshotPath(rootDirectory, monitor.Name));
        ProcessHelper.RunHyprctlProcess("focusmonitor", monitor.Name);
        break;

      default:
        ProcessHelper.RunHyprctlProcess("movetoworkspacesilent", $"special:1,title:Screenshot-Tool");
        grimGeometry = String.Empty;
        screenshotPath = ConstructScreenshotPath(rootDirectory, String.Empty);
        break;
    }

    Thread.Sleep(200);
    GrimScreenshot(screenshotPath, grimGeometry);
  }

  private static String ConstructScreenshotPath(String rootDirectory, String windowOrMonitorName)
  {
    if (!Directory.Exists(rootDirectory))
    {
      throw new DirectoryNotFoundException();
    }

    DateTime currentDateTime = DateTime.Now;

    String year = currentDateTime.Year.ToString();
    String month = ((Month)currentDateTime.Month).ToString();
    String dateTimeString = currentDateTime.ToString("yyyyMMdd_HHmmss");

    if (!String.IsNullOrEmpty(windowOrMonitorName))
    {
      windowOrMonitorName = $"_{windowOrMonitorName}";
    }

    String fileName = $"Screenshot_{dateTimeString}{windowOrMonitorName}.png";
    String screenshotDirectory = Path.Combine(rootDirectory, "Screenshots", year, month);

    if (!Directory.Exists(screenshotDirectory))
    {
      Directory.CreateDirectory(screenshotDirectory);
    }

    return Path.Combine(screenshotDirectory, fileName);
  }

  private static Boolean TryGetSelectionFromAvailable<T>(T[] items, Func<T, String> labelSelector, out T? result)
  {
    if (items is null || items.Length == 0)
    {
      throw new InvalidOperationException($"No {typeof(T).Name}s available to select.");
    }

    String[] labels = [.. items.Select(labelSelector)];
    Int32 selectedIndex = Menus.CreateMenu(labels);

    if (selectedIndex == -1)
    {
      result = default;
      return false;
    }
    result = items[selectedIndex];
    return true;
  }

  private static void GrimScreenshot(String screenshotPath, String geometry)
  {
    String[] arguments = String.IsNullOrEmpty(geometry)
      ? [screenshotPath] : ["-g", geometry, screenshotPath];

    using Process grimProcess = ProcessHelper.CreateProcess("grim", redirectStdIn: false, redirectStdOut: false, args: arguments);
    grimProcess.Start();
    grimProcess.WaitForExit();

    if (grimProcess.ExitCode != 0)
    {
      throw new InvalidOperationException($"grim failed with exit code {grimProcess.ExitCode}.");
    }

    CopyImageToClipboard(screenshotPath);
  }


  private static void CopyImageToClipboard(String screenshotPath)
  {
    using Process wlCopyProcess = ProcessHelper.CreateProcess("wl-copy", redirectStdIn: true, redirectStdOut: false, ["--type", "image/png"]);
    wlCopyProcess.Start();

    using FileStream fileStream = File.OpenRead(screenshotPath);
    fileStream.CopyTo(wlCopyProcess.StandardInput.BaseStream);

    wlCopyProcess.StandardInput.Close();
    wlCopyProcess.WaitForExit();

    if (wlCopyProcess.ExitCode != 0)
    {
      throw new InvalidOperationException($"wl-copy failed with exit code {wlCopyProcess.ExitCode}.");
    }
  }

  private static String GetSlurpGeometry()
  {
    using Process slurpProcess = ProcessHelper.CreateProcess("slurp", redirectStdIn: false, redirectStdOut: true, []);
    slurpProcess.Start();

    String slurpGeometry = slurpProcess.StandardOutput.ReadToEnd().Trim('\n');

    slurpProcess.WaitForExit();

    return String.IsNullOrEmpty(slurpGeometry)
      ? String.Empty : slurpGeometry;
  }
}
