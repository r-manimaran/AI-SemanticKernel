namespace WebApi;

public static class Utils
{
    public static void Separator()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("--------------------------------------------------");
        Console.ResetColor();
    }
    public static void WriteLineInformation(string message)
    {
        WriteLine(message, ConsoleColor.DarkGray);
    }
    public static void WriteLineError(string message)
    {
        WriteLine(message, ConsoleColor.Red);
    }
    public static void WriteLineWarning(string message)
    {
        WriteLine(message, ConsoleColor.Yellow);
    }
    public static void WriteLineSuccess(string message)
    {
        WriteLine(message, ConsoleColor.Green);
    }

    private static void WriteLine(string text, ConsoleColor color)
    {
        ConsoleColor currentColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }
        finally
        {
            Console.ForegroundColor = currentColor;
        }

    }


}