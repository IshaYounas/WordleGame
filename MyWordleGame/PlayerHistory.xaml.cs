using System.Text.Json;

namespace MyWordleGame
{
    public partial class PlayerHistory : ContentPage
    {
        // variables
        private const string historyFile = "history_file.json";
        private List<Progress> Items;

        // constructor
        public PlayerHistory()
        {
            InitializeComponent();
            Items = new List<Progress>(); // initialising items
            BindingContext = this;

            // calling custom methods
            LoadHistory();
        } // PlayerHistory
        // custom methods
        private void LoadHistory()
        {
            var localPath = Path.Combine(FileSystem.AppDataDirectory, historyFile);

            if (File.Exists(localPath))
            {
                var json = File.ReadAllText(localPath); // reading file
                var history = JsonSerializer.Deserialize<List<Progress>>(json); // deserialising it 

                Items.Clear(); // clearing 

                if (history != null)
                {
                    foreach (var item in history)
                        Items.Add(item); // adding item to Items list
                } // if
            } // if

            // updating the bindings
            OnPropertyChanged(nameof(Items));
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(IsNotEmpty));
        } // LoadHistory

        // checking if list is empty
        public bool IsEmpty()
        {
            return Items.Count == 0;
        } // IsEmpty

        // checking if list is NOT empty
        public bool IsNotEmpty()
        {
            return Items.Count > 0;
        } // IsNotEmpty
    } // class
} // namespace