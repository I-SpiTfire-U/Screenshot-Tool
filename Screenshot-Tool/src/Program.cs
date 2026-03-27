using Microsoft.VisualBasic.FileIO;
using Screenshot_Tool.src.Enums;

namespace Screenshot_Tool.src;

public class Program
{
  public static void Main(String[] args)
  {
    Console.CursorVisible = false;

    Int32 selectedOption = Menus.CreateMenu(["Area-Select", "Window", "Monitor", "Fullscreen"]);

    if (selectedOption == -1)
    {
      Console.CursorVisible = true;
      return;
    }

    ScreenshotType screenshotType = (ScreenshotType)selectedOption;
    ScreenShotting.TakeScreenshot(CustomDirectoryExists(args) ? args[0] : SpecialDirectories.MyPictures, screenshotType);

    Console.CursorVisible = true;
  }

  private static Boolean CustomDirectoryExists(String[] args) =>
    args.Length > 0 && Directory.Exists(args[0]);
}
