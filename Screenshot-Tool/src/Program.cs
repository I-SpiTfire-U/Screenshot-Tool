using System.Data;
using Microsoft.VisualBasic.FileIO;

namespace Screenshot_Tool.src;

public class Program
{
	public static void Main(string[] args)
	{
        Console.CursorVisible = false;

        string screenshotPath = ConstructScreenshotPath(args.Length > 0 ? args[0] : SpecialDirectories.MyPictures);

        Dictionary<string, Action> screenshotOptions = new()
        {
            { "Fullscreen",  ()=> Screenshot.ScreenshotFullscreen(screenshotPath) },
            { "Monitor",     ()=> Screenshot.ScreenshotMonitor(screenshotPath)    },
            { "Area-Select", ()=> Screenshot.ScreenshotAreaSelect(screenshotPath) },
            { "Window",      ()=> Screenshot.ScreenshotWindow(screenshotPath)     }
        };

        int i = Menus.CreateMenu(screenshotOptions.Keys);
        if (i >= 0)
        {
            screenshotOptions.ElementAt(i).Value.Invoke();
        }

        Console.CursorVisible = true;
    }

    private static string ConstructScreenshotPath(string baseDirectory)
    {
        string fileName = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}";
        string? month = Enum.GetName(Enum.Parse<Month>(DateTime.Now.Month.ToString()));
        string year = DateTime.Now.Year.ToString();

        if (string.IsNullOrEmpty(month))
        {
            throw new NoNullAllowedException();
        }

        string screenshotDirectory = Path.Combine(baseDirectory, "Screenshots", year, month);

        if (!Directory.Exists(screenshotDirectory))
        {
            Directory.CreateDirectory(screenshotDirectory);
        }

        return Path.Combine(screenshotDirectory, fileName);
    }
}