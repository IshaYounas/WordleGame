using System.Text.Json;

namespace MyWordleGame
{
    public partial class MainPage : ContentPage
    {
        // fields
        private const string LocalFile = "words.txt";
        private List<string> wordList = new List<string>();
        private string targetWord = "";
        private string currentGuess = "";
        private const int maxRows = 6;
        private const int maxCols = 5;
        private int currentRow = 0;

        // constructor
        public MainPage()
        {
            InitializeComponent();
        } // MainPage

        // moving the InitializeList() method call to OnAppearing
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await InitializeList();
        } // OnAppearing

        // methods for buttons - event handlers
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
            // if the word is not 5 letter long
            if (currentGuess.Length != maxCols)
            {
                await DisplayAlert("Error", "Guess a 5 letter word", "Ok");
                return;
            } // if 

            // if the word is not in the list
            if (!wordList.Contains(currentGuess.ToLower()))
            {
                await DisplayAlert("Error", "Word Not Found", "Ok");
                return;
            } // if

            // if the word is the target word
            if (currentGuess.ToLower() == targetWord.ToLower())
            {
                await CorrectWord();
                return;
            } // if

            UpdateRow();
            UpdateKeyColor();
            currentRow++; // moving to the next row

            // if the word is not guessed in 6 tries
            if (currentRow >= maxRows)
                await GameOver();// if

            currentGuess = "";
            UpdateGuessDisplay();
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

        public void Restart_Clicked(object sender, EventArgs e)
        {
            // resetting everything for the new game
            currentRow = 0;
            currentGuess = "";

            // clearing the grid
            foreach (var child in GameGrid.Children)
            {
                if (child is Label label)
                {
                    label.Text = "";
                    label.BackgroundColor = Colors.AntiqueWhite;
                } // if
            } // foreach

            // resetting key button colours
            foreach (var child in keyboard.Children)
            {
                if (child is Button button)
                    button.BackgroundColor = (Color)Application.Current.Resources["MidnightBlue"]; // if
            } // foreach

            if (wordList.Count > 0)
            {
                // randomizing a new target word 
                var word = new Random();
                targetWord = wordList[word.Next(wordList.Count)].ToLower();

                Console.WriteLine($"Target Word: {targetWord}");
            } // if

            else
                DisplayAlert("Error", "Word List Empty. Please try again", "OK"); // else
        } // Restart_Clicked

        private async void History_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PlayerHistory());
        } // History_Clicked

        private void Start_Clicked(object sender, EventArgs e)
        {
            string name1 = Player1Entry.Text.Trim();
            string name2 = Player2Entry.Text.Trim();

            // checking for empty inputs
            if (string.IsNullOrWhiteSpace(name1) || string.IsNullOrWhiteSpace(name2))
            {
                DisplayAlert("Input", "Names must be entered", "Ok");
                return;
            } // if

            DisplayAlert("Game Started", $"Welcome to Wordle, {name1} & {name2}", "Ok");

            Player1Entry.IsEnabled = false;
            Player2Entry.IsEnabled = false;
            Start.IsEnabled = false; // disabling once clicked
            Restart.IsEnabled = true; // enabling for restarting game
            History.IsEnabled = true;  // enabling history to be checked

            CreateGameGrid(); // creating the game grid
        } // Start_Clicked

        private void PlayerEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // checking if both enteries are filled
            bool isPlayer1 = !string.IsNullOrWhiteSpace(Player1Entry.Text);
            bool isPlayer2 = !string.IsNullOrWhiteSpace(Player2Entry.Text);

            // enabling start button - only if both input boxes have names in them
            Start.IsEnabled = isPlayer1 && isPlayer2;
        } // PlayerEntry_TextChanged

        // custom methods
        private void CreateGameGrid()
        {
            GameGrid.Children.Clear(); // clearing to reset the grid

            for (int row = 0; row < 6; row++) // 6 guesses
            {
                for (int col = 0; col < 5; col++) // 5 letter words
                {
                    var label = new Label
                    {
                        // setting the properties for the grid
                        BackgroundColor = Colors.AntiqueWhite,
                        FontSize = 20,
                        TextColor = Colors.Orchid,
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
                    label.Text = currentGuess[i].ToString().ToUpper(); // if

                // displaying an empty string
                else
                    label.Text = ""; // else
            } // for
        } // UpdateGuessDisplay

        private void UpdateRow()
        {
            var trackLetter = targetWord.ToCharArray(); // tracking letters that have been guessed
            var trackPosition = new bool[maxCols]; // tracking positions

            // checking for correct position - color green
            for (int i = 0; i < maxCols; i++)
            {
                var label = (Label)GameGrid.Children[currentRow * maxCols + i];
                label.Text = currentGuess[i].ToString().ToUpper(); // label text uppercase

                // checking if the guess matches the target word
                if (currentGuess[i].ToString().ToLower() == targetWord[i].ToString().ToLower())
                {
                    CorrectPosition(label); 
                    trackLetter[i] = '\0'; // storing letters that have been matched
                    trackPosition[i] = true; // storing the correct position
                } // if
            } // for

            // checking for correct letter but wrong position - color yellow
            for (int i = 0; i < maxCols; i++)
            { 
                // skippinng correct positions
                if (trackPosition[i])
                    continue;

                var label = (Label)GameGrid.Children[currentRow * maxCols + i];
                label.Text = currentGuess[i].ToString().ToUpper();

                if (Array.Exists(trackLetter, letter => letter.ToString().ToLower() == currentGuess[i].ToString().ToLower()))
                {
                    WrongPosition(label);

                    // finding the index of the currentGuess =in the trackLetter array
                    int index = Array.IndexOf(trackLetter, currentGuess[i].ToString().ToLower());
                    
                    if (index != -1)
                        trackLetter[index] = '\0'; // storing letters that have been matched
                } // if

                else
                    label.BackgroundColor = Colors.MidnightBlue;
            } // for
        } // UpdateRow

        private void UpdateKeyColor()
        {
            for (int i = 0; i < currentGuess.Length; i++)
            {
                foreach (var letter in currentGuess)
                {
                    foreach (var child in keyboard.Children)
                    {
                        if (child is Button button && button.Text == letter.ToString().ToUpper())
                        {
                            // setting the keyboard colours as it is in online game
                            if (targetWord.Contains(letter.ToString().ToLower()))
                            {
                                if (button.BackgroundColor != Colors.Green)
                                    button.BackgroundColor = Colors.Yellow; // if
                            }

                            else
                                button.BackgroundColor = Colors.MidnightBlue; // else
                        } // if
                    } // foreach
                } // foreach
            } // for

            for (int i = 0; i < currentGuess.Length;i++)
            {
                if (currentGuess[i].ToString().ToLower() == targetWord[i].ToString().ToLower())
                {
                    foreach (var child in keyboard.Children)
                    {
                        if (child is Button button && button.Text.ToUpper() == currentGuess[i].ToString().ToLower())
                            button.Background = Colors.Green;
                    } // foreach
                } // if
            } // for
        } // UpdateKeyColor

        private async Task GameOver()
        {
            bool tryAgain = await DisplayAlert("Bad Luck", $"Correct Word: {targetWord.ToUpper()}", "Try Again", "Exit");

            SaveHistory(currentRow + 1, targetWord);

            // passing the current object as sender and empty args
            if (tryAgain) // is true
                Restart_Clicked(this, EventArgs.Empty);// if

            else
            { 
                await DisplayAlert("Exit", "Thanks for playing", "Bye");
               // Application.Current.MainPage = new MainPage();
            } // else
        } // GameOver

        private void SaveHistory(int guesses, string correctWord)
        {
            var localPath = Path.Combine(FileSystem.AppDataDirectory, "history_file.json");

            // attempt is an object of the Progress class
            var attempt = new Progress
            {
                TimeStamp = DateTime.Now,
                CorrectWord = correctWord,
                Guesses = guesses,
                EmojiGrid = HistoryEmojiGrid()
            };

            // initialising history list & filling it with the appropriate data
            List<Progress> PlayerHistory = new List<Progress>();

            if (File.Exists(localPath))
            {
                string json = File.ReadAllText(localPath);

                // Serialised into a JSON file and loading would then be using deserialising - project requirement
                List<Progress>? desterilizeHistory = JsonSerializer.Deserialize<List<Progress>>(json);

                if (desterilizeHistory != null)
                    PlayerHistory = desterilizeHistory;  // if
            } // if

            PlayerHistory.Add(attempt); // adding the attempt to the history
            string jsonNew = JsonSerializer.Serialize(PlayerHistory); 
            File.WriteAllText(localPath, jsonNew);
        } // SaveHistory

        private string HistoryEmojiGrid()
        {
            string historyGrid = "";

            for (int row = 0; row <= currentRow; row++)
            {
                foreach (var label in RowLabel(row))
                {
                    // correct position
                    if (label.BackgroundColor == Colors.Green)
                        historyGrid += "🟩"; // if

                    // correct letter but wrong position
                    else if (label.BackgroundColor == Colors.Yellow)
                        historyGrid += "🟨"; // else if

                    // wrong letter
                    else
                        historyGrid += "⬛"; // else
                } // foreach

                historyGrid += "\n"; // next line
            } // for (row)

            return historyGrid.Trim();
        } // HistoryEmojiGrid

        // animation methods
        private async Task CorrectWord()
        {
            // changing background colour
            foreach (var label in RowLabel(currentRow))
            {
                // rotate and change bg colour
                await label.RotateTo(360, 1000);
                label.BackgroundColor = Colors.MistyRose;
            } // foreach

            bool playAgain = await DisplayAlert("Congrats!", "You Won!!!", "Play Again", "Exit");

            SaveHistory(currentRow + 1, targetWord);

            // passing the current object as sender and empty args
            if (playAgain)
                Restart_Clicked(this, EventArgs.Empty);// if

            else
            {
                await DisplayAlert("Exit", "Thanks for playing", "Bye");
                // Application.Current.MainPage = new MainPage();
            } // else
        } // CorrectWord

        private async void CorrectPosition(Label label)
        {
            // changing colour and zooming in & out
            label.BackgroundColor = Colors.Green;
            await label.ScaleTo(1.2, 200);
            await label.ScaleTo(1.0, 200);
        } // CorrectPosition

        private async void WrongPosition(Label label)
        {
            // changing colour and shaking up & down
            label.BackgroundColor = Colors.MintCream;
            await label.TranslateTo(0, -10, 100);
            await label.TranslateTo(0, 0, 100);
        } // WrongPosition

        private Label[] RowLabel(int row)
        {
            List<Label> labels = new List<Label>();

            foreach (var child in GameGrid.Children)
            {
                if (child is Label label && Grid.GetRow(label) == row)
                    labels.Add(label); // if
            } // foreach

            // returning the labels as an array
            return labels.ToArray();
        } // RowLabel

        // file reading methods
        private async Task InitializeList()
        {
            string localPath = Path.Combine(FileSystem.AppDataDirectory, LocalFile); // getting the full path for the local file

            if (!File.Exists(localPath))
                // downloading from online
                await DownloadFromOnline(localPath); // if 

            // reading the words from the file
            try
            {
                // opening the file for reading
                using (StreamReader read = new StreamReader(localPath))
                {
                    // reading file, spiltting into line, removing empty entries (whitespace) 
                    string content = await read.ReadToEndAsync();
                    wordList = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                } // read

                if (wordList.Count == 0)
                    await DisplayAlert("Error", "Word List Empty. Please try again", "OK"); // if

                else
                    TargetWord(); // else
            } // try

            catch (FileNotFoundException ex)
            {
                await DisplayAlert("Error", "File not found. Please try again.", "OK");
                Console.WriteLine($"FileNotFoundException: {ex.Message}");
            } // catch

            catch (IOException ex)
            {
                await DisplayAlert("Error", "Cannot access file. Please try again.", "OK");
                Console.WriteLine($"IOException: {ex.Message}");
            } // catch

            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not initialize the word list. Please try again.", "OK");
                Console.WriteLine($"Error: {ex.Message}");
            } // catch
        } // InitializeList

        private void TargetWord()
        {
            if (wordList.Count > 0)
            {
                // randomising a target word
                var word = new Random();
                targetWord = wordList[word.Next(wordList.Count)].ToLower();
                Console.WriteLine($"Target Word {targetWord}");
            } // if

            else
                DisplayAlert("Error", "Word List Empty. Please try again", "OK"); // else
        } // TargetWord
        private async Task DownloadFromOnline(string localPath)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";

                    // getting the content from the url (online file)
                    using (Stream response = await client.GetStreamAsync(url))

                    // creating a local file
                    using (FileStream localFile = new FileStream(localPath, FileMode.Create, FileAccess.Write))
                    {
                        await response.CopyToAsync(localFile);
                    } // file
                } // HttpClient

                Console.WriteLine("Download Successful!");
            } // try

            catch (Exception ex) // catching the exception
            {
                await DisplayAlert("Error", "Couldn't download the word list. Try again later.", "OK");
                Console.WriteLine($"Error: {ex.Message}");
            } // catch
        } // DownloadFromOnline
    } // class
} // namespace

