namespace MyWordleGame
{
    public class Progress
    {
        // variables
        public DateTime TimeStamp { get; set; }
        public string? CorrectWord { get; set; }
        public int Guesses { get; set; }
        public string? EmojiGrid { get; set; }

        // constructor
        public Progress()
        {

        } // Progress

        public Progress(DateTime timeStamp, string correctWord, int guesses, string emojiGrid)
        {
            TimeStamp = timeStamp;
            CorrectWord = correctWord;
            Guesses = guesses;
            EmojiGrid = emojiGrid;
        } // Progress
    } // class
} // namespace
