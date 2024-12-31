namespace MyWordleGame
{
    public class Player
    {
        // variables
        public string Name { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public List<string> GameHistory { get; set; }
        public string HistoryFile { get; }

        // constructor
        public Player(string name)
        {
            Name = name;
            GamesPlayed = 0;
            GamesWon = 0;
            GameHistory = new List<string>();
            HistoryFile = $"{Name}_Saved.txt";
        } // Player

        // custom methods
        public void SavePlayer()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(HistoryFile, false))
                {
                    writer.WriteLine(Name);
                    writer.WriteLine(GamesPlayed);
                    writer.WriteLine(GamesWon);

                    foreach (var gameResult in GameHistory)
                        writer.WriteLine(gameResult);
                } // writer
            } // try

            catch (Exception ex)
            {
                Console.WriteLine($"error saving data {ex.Message}");
            } // catch
        } // SavePlayer

        public static Player LoadPlayer(string name)
        {
            var player = new Player(name);

            if (File.Exists(player.HistoryFile))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(player.HistoryFile))
                    {
                        player.Name = reader.ReadLine();
                        player.GamesPlayed = int.Parse(reader.ReadLine());
                        player.GamesWon = int.Parse(reader.ReadLine());

                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            player.GameHistory.Add(line);
                        } // while
                    } // reader
                } // try

                catch (Exception ex)
                {
                    Console.WriteLine($"error loading player {ex.Message}");
                } // catch
            } // if

            return player;
        } // LoadPlayer
    } // class
} // namespace
