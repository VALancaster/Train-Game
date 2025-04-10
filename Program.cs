using CG_lab2;

class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game(1200, 1200))
        {
            game.Run();
        }
    }
}