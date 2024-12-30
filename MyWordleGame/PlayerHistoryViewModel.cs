using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;

namespace MyWordleGame
{
    public class PlayerHistoryViewModel : INotifyPropertyChanged
    {
        // variables
        private const string historyFile = "history_file.json";
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
                    // Serialised into a JSON file and loading would then be using deserialising. - project requirement
                    var json = File.ReadAllText(historyFile);
                    var history = JsonSerializer.Deserialize<ObservableCollection<Progress>>(json);

                    if (history != null)
                        Items = history; // if
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
                // writing text to the file
                var json = JsonSerializer.Serialize(Items);
                File.WriteAllText(historyFile, json);
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
