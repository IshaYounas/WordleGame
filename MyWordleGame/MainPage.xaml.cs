using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
//using Plugin.Maui.Audio;

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
        private bool isPlayer1Turn = true;
        public PlayerHistoryViewModel HistoryViewModel { get; set; }

        private Player player1;
        private Player player2;

        // audio files
        //private IAudioPlayer audioPlayerFullCorrect;
        //private IAudioPlayer audioPlayerYellowsAndGreens;
        //private IAudioPlayer audioPlayerFullWrong;

        // constructor
        public MainPage()
        {
            InitializeComponent();
            HistoryViewModel = new PlayerHistoryViewModel();
            BindingContext = this;
        } // MainPage

        // moving the InitializeList() method call to OnAppearing
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await InitializeList();
            //await LoadAudioFiles(); // crashing with audio files
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
                UpdatePlayer();

                if (isPlayer1Turn)
                    PlayerTurn.Text = $"{Player1Entry.Text} Wins"; // if

                else
                    PlayerTurn.Text = $"{Player2Entry.Text} Wins"; // else

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

            if (isPlayer1Turn)
            {
                isPlayer1Turn = false;
                PlayerTurn.Text = $"{Player2Entry.Text}'s Turn";
            } // if

            else
            {
                isPlayer1Turn = true;
                PlayerTurn.Text = $"{Player1Entry.Text}'s Turn";
            } // else

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
            isPlayer1Turn = true;

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
            /*foreach (var child in keyboard.Children)
            {
                if (child is Button button)
                    button.BackgroundColor = (Color)Application.Current.Resources["Salmon"]; // if 
            } // foreach */

            if (wordList.Count > 0)
            {
                // randomizing a new target word 
                var word = new Random();
                targetWord = wordList[word.Next(wordList.Count)].ToLower();

                Console.WriteLine($"Target Word: {targetWord}");
            } // if

            else
                DisplayAlert("Error", "Word List Empty. Please try again", "OK"); // else

            if (isPlayer1Turn)
                PlayerTurn.Text = "Player 1's Turn"; // if

            else
                PlayerTurn.Text = "Player 2's Turn"; // else
        } // Restart_Clicked

        private async void History_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PlayerHistory());
        } // History_Clicked

        private void Start_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Player1Entry.Text))
                player1 = Player.LoadPlayer(Player1Entry.Text); // if

            else
                player1 = Player.LoadPlayer("Player 1"); // else

            if (!string.IsNullOrWhiteSpace(Player2Entry.Text))
                player2 = Player.LoadPlayer(Player2Entry.Text); // if

            else
                player2 = Player.LoadPlayer("Player 2"); // else

            player1.SavePlayer();
            player2.SavePlayer();

            Player1Entry.IsEnabled = false;
            Player2Entry.IsEnabled = false;
            Start.IsEnabled = false; // disabling once clicked
            Restart.IsEnabled = true; // enabling for restarting game
            History.IsEnabled = true;  // enabling history to be checked

            CreateGameGrid(); // creating the game grid
            CreateKeyboard(); // creating keyboard grid
        } // Start_Clicked

        private void PlayerEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Player1Entry.Text) && (!string.IsNullOrWhiteSpace(Player2Entry.Text)))
                 Start.IsEnabled = true; // if

            else 
                Start.IsEnabled = false; // else
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
                        TextColor = Colors.DarkTurquoise,
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

        private void CreateKeyboard()
        {
            keyboard.Children.Clear(); // clearing to reset the grid

            string[] keyboardRows = new string[] { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };

            for (int i = 0; i < 10; i++)
            {
                keyboard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            } // for (i)

            for (int row = 0; row < keyboardRows.Length; row++)
            {
                string rowKeys = keyboardRows[row];
                double startcol = 0; // start col position

                if (row == 0)
                    startcol = 0.5;

                if (row == 1)
                    startcol = 1;

                if (row == 2)
                    startcol = 1.5;

                for (int col = 0; col < rowKeys.Length; col++)
                {
                    var button = new Button
                    {
                        // setting the properties for the grid
                        Text = rowKeys[col].ToString(),
                        BackgroundColor = Colors.LightSlateGray,
                        FontSize = 16,
                        TextColor = Colors.SaddleBrown,
                        FontAttributes = FontAttributes.Bold,
                        HeightRequest = 50,
                        WidthRequest = 50
                    }; // var button

                    button.Clicked += Key_Clicked;

                    // adding button to the grid position
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, (int)(startcol + col));
                    keyboard.Children.Add(button);
                } // for (col)

                // adding space
                keyboard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            } // for (row)

            // adding enter & delete button
            var enterBtn = new Button
            {
                Text = "Enter",
                BackgroundColor = Colors.LightGray,
                TextColor = Colors.DarkGoldenrod,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 50,
                WidthRequest = 100,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }; // enterBtn

            enterBtn.Clicked += Enter_Clicked;

            var deleteBtn = new Button
            {
                Text = "Delete",
                BackgroundColor = Colors.LightGray,
                TextColor = Colors.DarkGoldenrod,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 50,
                WidthRequest = 100,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }; // deleteBtn

            deleteBtn.Clicked += Del_Clicked;

            // setting row, column and column span position
            keyboard.Children.Add(enterBtn);
            Grid.SetRow(enterBtn, keyboardRows.Length);
            Grid.SetColumn(enterBtn, 0);
            Grid.SetColumnSpan(enterBtn, 5);

            keyboard.Children.Add(deleteBtn);
            Grid.SetRow(deleteBtn, keyboardRows.Length);
            Grid.SetColumn(deleteBtn, 5);
            Grid.SetColumnSpan(deleteBtn, 5);
        } // CreateKeyboard

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
                {
                    label.BackgroundColor = Colors.Salmon;
                    //audioPlayerFullWrong.Play();
                } // else
            } // for
        } // UpdateRow

        // cannot get the keyboard colour to change 
        private void UpdateKeyColor()
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
                            button.BackgroundColor = Colors.Salmon; // else
                    } // if
                } // foreach
            } // foreach

            for (int i = 0; i < currentGuess.Length;i++)
            {
                if (currentGuess[i].ToString().ToLower() == targetWord[i].ToString().ToLower())
                {
                    foreach (var child in keyboard.Children)
                    {
                        if (child is Button button && button.Text == currentGuess[i].ToString().ToUpper())
                        {
                            button.BackgroundColor = Colors.Green; 
                        } // if
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
                await DisplayAlert("Exit", "Thanks for Playing", "Ok");
                Application.Current.Quit();
            } // else
        } // GameOver

        private void SaveHistory(int guesses, string correctWord)
        {
            var mainPage = Application.Current.MainPage as MainPage;
            if (mainPage != null)
            {
                var viewModel = mainPage.HistoryViewModel;

                var attempt = new Progress
                {
                    TimeStamp = DateTime.Now,
                    CorrectWord = correctWord,
                    Guesses = guesses,
                    EmojiGrid = HistoryEmojiGrid()
                }; // attempt

                viewModel.NewAttempt(attempt);
            } // if

            else
                Console.WriteLine("MainPage cannot be accessed");
        } // SaveHistory

        private string HistoryEmojiGrid()
        {
            string historyGrid = "";

            for (int row = 0; row <= currentRow; row++)
            {
                foreach (var label in RowLabel(row))
                {
                    // correct position - grren
                    if (label.BackgroundColor == Colors.Green)
                        historyGrid += "🟩"; // if

                    // correct letter but wrong position - yellow
                    else if (label.BackgroundColor == Colors.Yellow)
                        historyGrid += "🟨"; // else if

                    // wrong letter - salmon
                    else
                        historyGrid += "🟧"; // else
                } // foreach

                historyGrid += "\n"; // next line
            } // for (row)

            return historyGrid.Trim();
        } // HistoryEmojiGrid

        private void UpdatePlayer()
        {
            if (isPlayer1Turn)
                player1.GamesWon++;

            else
                player2.GamesWon++;

            player1.GamesPlayed++;
            player2.GamesPlayed++;
            player1.SavePlayer();
            player2.SavePlayer();
        } // UpdatePlayer

        // animation methods
        private async Task CorrectWord()
        {
            var labels = RowLabel(currentRow);
            if (labels == null | labels.Length == 0)
            {
                Console.WriteLine("No labels found");
                return;
            } // if

            // changing background colour
            foreach (var label in RowLabel(currentRow))
            {
                // rotate and change bg colour
                await label.RotateTo(360, 1000);
                label.BackgroundColor = Colors.Green;
                //audioPlayerFullCorrect.Play();
            } // foreach

            string winner;

            if (isPlayer1Turn)
                winner = player1.Name; // if

            else
                winner = player2.Name; // else

            bool playAgain = await DisplayAlert("Congrats!", $"{winner} Won!!!", "Play Again", "Exit");

            SaveHistory(currentRow + 1, targetWord);
            UpdatePlayer();

            // passing the current object as sender and empty args
            if (playAgain)
                Restart_Clicked(this, EventArgs.Empty);// if

            else
            {
                await DisplayAlert("Exit", "Thanks for Playing", "Ok");
                Application.Current.Quit();
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
            label.BackgroundColor = Colors.Yellow;
            await label.TranslateTo(0, -10, 100);
            await label.TranslateTo(0, 0, 100);
            //audioPlayerYellowsAndGreens.Play();
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

        /* 
         // crashing with audio files
        private async Task LoadAudioFiles()
        {
            try
            {
                // Loading files when the app opens from resources
                audioPlayerFullCorrect = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("Resources/AudioFiles/FullCorrectWords.mp3"));
                audioPlayerYellowsAndGreens = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("Resources/AudioFiles/YellowsGreens.mp3"));
                audioPlayerFullWrong = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("Resources/AudioFiles/FullWrongWord.mp3"));
            } // try

            catch (Exception ex)
            {
                await DisplayAlert("error", "failed: " + ex.Message, "ok");
            } // catch 
        } // LoadAudioFiles */
    } // class
} // namespace

