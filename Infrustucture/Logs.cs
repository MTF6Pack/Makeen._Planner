namespace Infrustucture
{
    public static class Logs
    {
        public static void Write(string message)
        {
            message = DateTime.Now + " | " + message + "\n" + new string('-', 100) + "\n";
            Console.WriteLine(message);
            File.AppendAllText("../Makeen._Planne/Logs/Logs.txt", message);
        }
        public static void WriteYellow(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Write(message);
            Console.ResetColor();
        }
        public static void WriteError(string message)
        {
            message = DateTime.Now + " | " + message + "\n" + new string('-', 100) + "\n";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(DateTime.Now + " | " + message + "\n" + new string('-', 100));
            Console.ResetColor();
            File.AppendAllText("../Makeen._Planner/Logs/Errors.txt", message);
        }
    }
}
