namespace Screenshot_Tool.src;

public static class Menus
{
  public static Int32 CreateMenu(String[] options)
  {
    Int32 index = 0;

    while (true)
    {
      Display(options, index);

      switch (Console.ReadKey(true).Key)
      {
        case ConsoleKey.UpArrow:
          index = Math.Max(index - 1, 0);
          break;

        case ConsoleKey.DownArrow:
          index = Math.Min(index + 1, options.Length - 1);
          break;

        case ConsoleKey.Enter:
          return index;

        case ConsoleKey.Q:
        case ConsoleKey.Escape:
          return -1;
      }
    }
  }

  private static void Display(String[] options, Int32 index)
  {
    Console.Clear();

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(" -Screenshot-");
    for (Int32 i = 0; i < options.Length; i++)
    {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.Write(index == i ? "> " : ' ');
      Console.ResetColor();

      Console.WriteLine(options[i]);
    }
  }
}
