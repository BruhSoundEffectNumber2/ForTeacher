namespace ForTeacher
{
    public static class Debug
    {
        #region Console
        public static void Log(string message, string type = "Log")
        {
            string full = type + " -- " + message;
            Console.WriteLine(full);
            Console.ResetColor();
        }

        public static void Warning(string message, string type = "Log")
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Log(message, type);
        }

        public static void Error(string message, string type = "Log")
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Log(message, type);
        }

        public static void Break()
        {
            Console.WriteLine("--------------------");
        }
        #endregion
    }
}
