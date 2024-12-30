using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.IO;

namespace MyWordleGame
{
    public class PlayerHistoryViewModel : INotifyPropertyChanged
    {
        // variables
        private const string historyFile = "history_file.txt";
        private ObservableCollection<Progress> _items;

        public ObservableCollection<Progress> Items
        {
            // accessing _items
            get => _items; // get

            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(IsNotEmpty));
            } // set
        } // ObservableCollection

        // properties
        public bool IsEmpty => Items == null || Items.Count == 0;
        public bool IsNotEmpty => !IsEmpty;

        // constructor
        public PlayerHistoryViewModel()
        {
            Items = new ObservableCollection<Progress>(); // initialising Items 
            LoadHistory();
        } // PlayerHistoryViewModel

        private void LoadHistory()
        {
            if (File.Exists(historyFile))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(historyFile))
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('\t');

                            if (parts.Length == 4)
                            {
                                DateTime timeStamp = DateTime.Parse(parts[0]);
                                string correctWord = parts[1];
                                int guesses = int.Parse(parts[2]);
                                string emojiGrid = parts[3];

                                var progress = new Progress(timeStamp, correctWord, guesses, emojiGrid);
                                Items.Add(progress);
                            } // if
                        } // while
                    } // reader
                } // try

                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading history: {ex.Message}");
                } // catch
            } // if
        } // LoadHistory

        public void SaveHistory()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(historyFile, false)) // appending
                {
                    foreach (var item in Items)
                    {
                        // writing to the file
                        string line = $"{item.TimeStamp:O}\n{item.CorrectWord}\n{item.Guesses}\n{item.EmojiGrid}";
                        writer.WriteLine(line);
                    } // foreach
                } // writer
            } // try

            catch (Exception ex)
            {
                Console.WriteLine($"Error saving history: {ex.Message}");
            } // catch
        } // SaveHistory

        public void NewAttempt(Progress progress)
        {
            Items.Add(progress);
            SaveHistory();
        } // NewAttempt

        // invoking the PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } // OnPropertyChanged
    } // class
} // namespace