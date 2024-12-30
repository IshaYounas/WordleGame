namespace MyWordleGame
{
    public partial class App : Application
    {
        public PlayerHistoryViewModel HistoryViewModel { get; } 
        public App()
        {
            InitializeComponent();

            // MainPage = new AppShell();
            HistoryViewModel = new PlayerHistoryViewModel();
            MainPage = new NavigationPage(new MainPage());
        }
    }
}
