using Plugin.Maui.Audio;
namespace MyWordleGame
{
    public partial class MainPage : ContentPage
    {
        // fields
        private const string LocalFile = "words.txt";
        private const string OnlineFile = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
        private List<string> list = new List<string>();
        private string targetWord = "";
        private string currentGuess = "";
        private const int maxRows = 6;
        private const int maxCols = 5;
        private int currentRow = 0;

        // constructor
        public MainPage()
        {
            InitializeComponent();

            // calling custom methods
            InitializeList();
            CreateGameGrid();
        } // MainPage

        // methods for buttons
        private void Key_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button != null && currentGuess.Length < maxCols)
            {
                currentGuess += button.Text; // adding the guess letter to the input box
                UpdateGuessDisplay(); // calling the method
            } // if
        } // Key_Clicked

        private async void Enter_Clicked(object sender, EventArgs e)
        {
            var label = (Label)GameGrid.Children[currentRow * maxCols + 1];

            if (currentGuess.Length == maxCols && currentRow < maxRows)
            {
                // if the word is not in the list
                if (!list.Contains(currentGuess))
                {
                    await DisplayAlert("Invalid Word", "Word Not Found", "Ok");
                    return;
                } // if

                // if the word is the target word
                if (currentGuess == targetWord)
                {
                    await DisplayAlert("Congrats", "You guessed the correct word!", "Ok");
                    return;
                } // if

                for (int i = 0; i < maxCols; i++)
                {
                    // setting the colours as it is in  online game

                    if (currentGuess[i] == targetWord[i])
                        label.BackgroundColor = Colors.Green; // if

                    else if (targetWord.Contains(currentGuess[i]))
                        label.BackgroundColor = Colors.Yellow; // else if

                    else
                        label.BackgroundColor = Colors.Gray; 
                } // for

                // moving to the next row
                currentRow++;
                currentGuess = "";
                UpdateGuessDisplay();
            } // if
        } // Enter_Clicked

        private void Del_Clicked(object sender, EventArgs e)
        {
            if (currentGuess.Length > 0)
            {
                // removing the last letter
                currentGuess = currentGuess.Substring(0, currentGuess.Length - 1);
                UpdateGuessDisplay();
            } // if
        } // Del_Clicked

        // custom methods
        private void CreateGameGrid()
        {
            for (int row = 0; row < 6; row++) // 6 guesses
            {
                for (int col = 0; col < 5; col++) // 5 letter words
                {
                    var label = new Label
                    {
                        // setting the properties for the grid
                        BackgroundColor = Colors.AntiqueWhite,
                        FontSize = 20,
                        TextColor = Colors.DarkBlue,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        HeightRequest = 50,
                        WidthRequest = 50
                    }; // var label

                    // adding label to the grid position
                    Grid.SetRow(label, row);
                    Grid.SetColumn(label, col);
                    GameGrid.Children.Add(label);
                } // for (col)
            } // for (row)
        } // CreateGameGrid

        private void UpdateGuessDisplay()
        {
            // updating the current row
            for (int i = 0; i < maxCols; i++)
            {
                // placing the label in the current row
                var label = (Label)GameGrid.Children[currentRow * maxCols + i]; 

                // displaying the letter
                if (i < currentGuess.Length)
                    label.Text = currentGuess[i].ToString(); // if

                // displaying an empty string
                else
                    label.Text = ""; // else
            } // for
        } // UpdateGuessDisplay

        private async void InitializeList()
        {
            string localPath = Path.Combine(FileSystem.AppDataDirectory, LocalFile); // getting the full path for the local file

            // calling the appropriate methods
            // reading the words from the file
            if (File.Exists(localPath))
                list = await ReadFromFile(localPath); // if

            else
            { 
                // downloading the file to the local path
                await DownloadFromOnline(localPath);
                list = await ReadFromFile(localPath);
            } // else

            // randomly selecting a word for the current game
            if (list.Count > 0)
            {
                Random word = new Random();
                targetWord = list[word.Next(list.Count)];
            } // if
        } // InitializeList

        private async Task DownloadFromOnline(string localPath)
        {
            using (HttpClient client = new HttpClient()) // getting the word for the word list
            {
                try
                {
                    // throwing an exception if the the http request fails
                    var response = await client.GetAsync(OnlineFile);
                    response.EnsureSuccessStatusCode(); 

                    // writing the downloaded list to the file 
                    var wordsOnline = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync(localPath, wordsOnline);

                } // try

                catch (Exception ex) // catching the exception
                {
                    Console.WriteLine($"Error downloading words: {ex.Message}");
                } // catch
            } // HttpClient client
        } // DownloadFromOnline

        private async Task<List<string>> ReadFromFile(string localPath)
        {
            try
            {
                var wordsFile = await File.ReadAllTextAsync(localPath); // string
                // splitting the stings
                return wordsFile.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList(); 
            } // try

            catch (Exception ex)
            {
                Console.WriteLine($"Error reading words from file: {ex.Message}");
                return new List<string>();
            } // catch
        } // ReadFromFile
    } // class
} // namespace
