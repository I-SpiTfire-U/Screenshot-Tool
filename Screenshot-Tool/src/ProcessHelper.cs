using System.Diagnostics;

namespace Screenshot_Tool.src;

public static class ProcessHelper
{
  public static Process CreateProcess(String name, Boolean redirectStdIn, Boolean redirectStdOut, String[] args)
  {
    return new()
    {
      StartInfo = new ProcessStartInfo(name, args)
      {
        UseShellExecute = false,
        RedirectStandardInput = redirectStdIn,
        RedirectStandardOutput = redirectStdOut,
        RedirectStandardError = false,
        CreateNoWindow = true
      }
    };
  }

  public static void RunHyprctlProcess(params String[] arguments)
  {
    using Process hyprctlProcess = CreateProcess("hyprctl", redirectStdIn: false, redirectStdOut: false, ["dispatch", .. arguments]);
    hyprctlProcess.Start();

    hyprctlProcess.WaitForExit();
    Thread.Sleep(300);
  }
}
