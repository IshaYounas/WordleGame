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
            CreateGameGrid();
        } // MainPage

        // moving the InitializeList() method call to OnAppearing
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await InitializeList();
        } // OnAppearing

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
            // if the word is not 5 letter long
            if (currentGuess.Length != maxCols)
            {
                Console.WriteLine("Guess is not 5 letters long.");
                await DisplayAlert("Error", "Guess a 5 letter word", "Ok");
                return;
            } // if 

            // if the word is not in the list
            if (!wordList.Contains(currentGuess.ToLower()))
            {
                Console.WriteLine($"Word not found: {currentGuess}");
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

        private void UpdateRow()
        {
            var trackLetter = targetWord.ToCharArray(); // tracking letters that have been guessed
            var trackPosition = new bool[maxCols]; // tracking positions

            // checking for correct positions - color green
            for (int i = 0; i < maxCols; i++)
            {
                var label = (Label)GameGrid.Children[currentRow * maxCols + i];
                label.Text = currentGuess[i].ToString().ToUpper();

                if (currentGuess[i] == targetWord[i])
                {
                    CorrectPosition(label);
                    trackLetter[i] = '\0'; // storing letters that have been matched
                    trackPosition[i] = true; // storing the correct position
                } // if
            } // for

            // checking for correct letter but wrong position - color green
            for (int i = 0; i < maxCols; i++)
            {
                if (!trackPosition[i])
                {
                    var label = (Label)GameGrid.Children[currentRow * maxCols + i];

                    if (trackLetter.Contains(currentGuess[i]))
                    {
                        WrongPosition(label);
                        trackLetter[Array.IndexOf(trackLetter, currentGuess[i])] = '\0'; // letter in the word
                    } // if

                    else
                        label.BackgroundColor = Colors.Gray; // letter is not in the word
                } // if
            } // for
        } // UpdateRow

        private void UpdateKeyColor()
        {
            foreach (var letter in currentGuess)
            {
                foreach (var child in keyboard.Children)
                {
                    if (child is Button button && button.Text == letter.ToString().ToUpper())
                    {
                        // setting the keyboard colours as it is in online game
                        if (targetWord.Contains(letter))
                        {
                            if (targetWord.IndexOf(letter) == currentGuess.IndexOf(letter))
                                button.BackgroundColor = Colors.Green; // if

                            else if (button.BackgroundColor != Colors.Green)
                                button.BackgroundColor = Colors.Yellow; // else if
                        } // if

                        else
                        {
                            if (button.BackgroundColor != Colors.Green && button.BackgroundColor != Colors.Yellow)
                                button.BackgroundColor = Colors.Gray; // if
                        } // else
                    } // if
                } // foreach
            } // foreach
        } // UpdateKeyColor

        private async void RestartGame()
        {
            currentRow = 0;
            currentGuess = "";

            /// clearing the grid
            foreach (var child in GameGrid.Children)
            {
                if (child is Label label)
                {
                    label.Text = "";
                    label.BackgroundColor = Colors.AntiqueWhite;
                } // if
            } // foreach
        } // RestartGame

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

            await DisplayAlert("Congrats!", "You Won!!!", "Ok");
        } // CorrectWord

        private async Task GameOver()
        {
            Console.WriteLine($"Game Over: Target Word was: {targetWord}");

            // change bg colour and display a pop up alert
            BackgroundColor = Colors.Yellow;
            await DisplayAlert("No Good", $"Correct Word: {targetWord.ToUpper()}", "Try Again");
        } // GameOver

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
            Console.WriteLine($"Local File: {localPath}");


            if (!File.Exists(localPath))
            {
                Console.WriteLine("Downloading file");
                await DownloadFromOnline(localPath); // downloading from online
            } // if 

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

                if (wordList.Count > 0)
                {
                    // randomizing a taget word 
                    var word = new Random();
                    targetWord = wordList[word.Next(wordList.Count)].ToLower();

                    Console.WriteLine($"Target Word: {targetWord}");
                } // if

                else
                {
                    await DisplayAlert("Error", "Word List Empty.", "OK");
                }
            } // try

            catch (Exception ex)
            {
                Console.WriteLine($"Error loading words: {ex.Message}");
                await DisplayAlert("Error", "Could not initialize the word list. Please try again.", "OK");
            } // catch
        } // InitializeList

        private async Task DownloadFromOnline(string localPath)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
                    Console.WriteLine($"Downloaing from URL {url}");

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
                Console.WriteLine($"Error downloading words: {ex.Message}");
                await DisplayAlert("Error", "Couldn't download the word list. Try again later.", "OK");
            } // catch
        } // DownloadFromOnline
    } // class
} // namespace
