using System.Diagnostics;

namespace Screenshot_Tool.src;

public static class Screenshot
{
    public static void ScreenshotFullscreen(string screenshotPath)
    {
        HideWindow();
        GrimScreenshot(screenshotPath);
    }

    public static void ScreenshotMonitor(string screenshotPath)
    {
        MonitorInfo[] availableMonitors = Monitors.GetAvailableMonitors();
        int selectedMonitor = Menus.CreateMenu([.. availableMonitors.Select(m => m.Name)]);

        if (selectedMonitor < 0)
        {
            return;
        }

        HideWindow();
        GrimScreenshot(screenshotPath, availableMonitors[selectedMonitor].GetGeometryAsString());
    }

    public static void ScreenshotAreaSelect(string screenshotPath)
    {
        HideWindow();
        GrimScreenshot(screenshotPath, GetSlurpGeometry());
    }

    public static void ScreenshotWindow(string screenshotPath)
    {
        WindowInfo[] availableWindows = Windows.GetAvailableWindows();
        int selectedWindow = Menus.CreateMenu([.. availableWindows.Select(m => m.Class)]);

        if (selectedWindow < 0)
        {
            return;
        }

        string windowClass = availableWindows[selectedWindow].Class;
        string windowName = windowClass.Contains('.')
            ? windowClass.Split('.').Last()
            : windowClass;

        string fileName = $"{Path.GetFileNameWithoutExtension(screenshotPath)}_{windowName}";
        string fileExtension = Path.GetExtension(screenshotPath);
        string directoryName = Path.GetDirectoryName(screenshotPath)!;

        screenshotPath = Path.Combine(directoryName, $"{fileName}{fileExtension}");

        HideWindow();
        FocusWindow(windowClass);
        GrimScreenshot(screenshotPath, availableWindows[selectedWindow].GetGeometryAsString());
    }

    private static void GrimScreenshot(string resultPath, string? geometry = null, bool copyToClipboard = true)
    {
        if (geometry == string.Empty)
        {
            return;
        }

        string pngPath = $"{resultPath}.png";

        string[] arguments = geometry is not null
            ? ["-g", geometry, pngPath]
            : [pngPath];
        using Process grimProcess = ProcessHelper.CreateProcess("grim", redirectStdIn: false, redirectStdOut: false, args: arguments);

        grimProcess.Start();
        grimProcess.WaitForExit();

        if (grimProcess.ExitCode != 0)
        {
            throw new InvalidOperationException($"grim failed with exit code {grimProcess.ExitCode}.");
        }

        if (copyToClipboard)
        {
            CopyImageToClipboard($"{resultPath}.png");
        }
    }


    private static void CopyImageToClipboard(string pngPath)
    {
        string[] arguments = ["--type", "image/png"];
        using Process wlCopyProcess = ProcessHelper.CreateProcess("wl-copy", redirectStdIn: true, redirectStdOut: false, args: arguments);

        wlCopyProcess.Start();

        using (FileStream fs = File.OpenRead(pngPath))
        {
            fs.CopyTo(wlCopyProcess.StandardInput.BaseStream);
        }

        wlCopyProcess.StandardInput.Close();
        wlCopyProcess.WaitForExit();

        if (wlCopyProcess.ExitCode != 0)
        {
            throw new InvalidOperationException($"wl-copy failed with exit code {wlCopyProcess.ExitCode}.");
        }
    }

    private static string? GetSlurpGeometry()
    {
        using Process slurpProcess = ProcessHelper.CreateProcess("slurp", redirectStdIn: false, redirectStdOut: true, args: []);

        slurpProcess.Start();

        string slurpGeometry = slurpProcess.StandardOutput.ReadToEnd();

        slurpProcess.WaitForExit();

        return !string.IsNullOrWhiteSpace(slurpGeometry)
            ? slurpGeometry.Trim('\n')
            : string.Empty;
    }

    private static void HideWindow()
    {
        string[] arguments =
        [
            "dispatch",
            "movetoworkspacesilent",
            "special:1,class:screenshot-tool",
        ];

        using Process hyprctlProcess = ProcessHelper.CreateProcess("hyprctl", redirectStdIn: false, redirectStdOut: false, args: arguments);
        
        hyprctlProcess.Start();
        hyprctlProcess.WaitForExit();
        
        Thread.Sleep(300);
    }

    private static void FocusWindow(string windowClass)
    {
        string[] arguments =
        [
            "dispatch",
            "focuswindow",
            $"class:{windowClass}",
        ];

        using Process hyprctlProcess = ProcessHelper.CreateProcess("hyprctl", redirectStdIn: false, redirectStdOut: false, args: arguments);
        
        hyprctlProcess.Start();
        hyprctlProcess.WaitForExit();

        Thread.Sleep(300);
    }
}