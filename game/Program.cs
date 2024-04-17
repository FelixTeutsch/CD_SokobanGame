using libs;

class Program
{
    static void Main(string[] args)
    {
        //Setup
        Console.CursorVisible = false;
        var engine = GameEngine.Instance;
        var inputHandler = InputHandler.Instance;


        while (engine.LoadNextLevel())
        {
            // Main game loop
            while (!engine.IsGameWon())
            {
                engine.Render();
                // Handle keyboard input
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                bool skipCollisionCheck = inputHandler.Handle(keyInfo);
                if (skipCollisionCheck)
                    continue;

                // check collision
                engine.CheckCollision();
            }
            engine.Render();
            Console.WriteLine("You won!\nPress any key to continue to the next level...");
            Console.ReadKey();
        }
        engine.Render();
        Console.WriteLine("You won!");
    }
}