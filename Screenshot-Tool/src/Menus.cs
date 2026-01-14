namespace Screenshot_Tool.src;

public static class Menus
{
    public static int CreateMenu(ICollection<string> options)
    {
        int index = 0;

        while (true)
        {
            Display(options, index);

            ConsoleKey userInput = Console.ReadKey(true).Key;
            switch (userInput)
            {
                case ConsoleKey.UpArrow:
                    index = Math.Max(index - 1, 0);
                    break;

                case ConsoleKey.DownArrow:
                    index = Math.Min(index + 1, options.Count - 1);
                    break;

                case ConsoleKey.Enter:
                    return index;

                case ConsoleKey.Q:
                case ConsoleKey.Escape:
                    return -1;
            }
        }
    }

    private static void Display(ICollection<string> options, int index)
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(" -Screenshot-");
        for (int i = 0; i < options.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(index == i ? "> " : ' ');
            Console.ResetColor();

            Console.WriteLine(options.ElementAt(i));
        }
    }
}