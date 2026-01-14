using System.Diagnostics;

namespace Screenshot_Tool.src;

public static class ProcessHelper
{
    public static Process CreateProcess(string name, bool redirectStdIn, bool redirectStdOut, params string[] args)
    {
        ProcessStartInfo processStartInfo = new(name, args)
        {
            UseShellExecute = false,
            RedirectStandardInput = redirectStdIn,
            RedirectStandardOutput = redirectStdOut,
            RedirectStandardError = false,
            CreateNoWindow = true
        };

        return new()
        {
            StartInfo = processStartInfo
        };
    }
}