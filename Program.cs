using ComputerGraphics_lab2;

class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game(1800, 1200))
        {
            game.Run();
        }
    }
}