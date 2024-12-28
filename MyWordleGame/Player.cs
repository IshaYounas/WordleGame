namespace MyWordleGame
{
    public class Player
    {
        // variables
        public string Name { get; set; }
        public int Score { get; set; }
        private string filePath => $"{Name}.txt";

        // constructor
        public Player(string name)
        {
            this.Name = name;

            // calling methods
            if (File.Exists(filePath))
                LoadPlayer(); // previous player

            else
                NewPlayer();
        } // Player

        // custom methods
        private void LoadPlayer() // loading a saved player
        {
            Console.WriteLine($"Welcome back, {Name}!");

            try
            {
                string data = File.ReadAllText(filePath);

                // replacing the data string with "Score:"
                if (int.TryParse(data.Replace("Score: ", ""), out int score))  // converting string into an integer
                    Score = score; // if

                else
                    score = 0; // else
            } // try

            catch (Exception ex) 
            {
                Console.WriteLine($"Error reading score, {ex.Message}");
                Score = 0;
            } // else
        } // LoadPlayer

        private void NewPlayer() // creating a new player
        {
            Console.Write($"Welcome to the game of Wordle, {Name}");
            Score = 0;
            SavePlayer();
        } // NewPlayer

        public void SavePlayer() // saving to a file
        {
            File.WriteAllText(filePath, $"Score: {Score}");
        } // SavePlayer

        public void UpdateScore(int points) // updating score
        {
            Score += points;
            SavePlayer();
        } // UpdateScore
    } // class
} // namespace
