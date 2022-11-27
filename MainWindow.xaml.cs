using System.Linq;
using System.Windows;

namespace TRPO_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using var db = new AttorneyContext();
            _ = db.Case.FirstOrDefault();
        }

        private void ToClientsButton_Click(object sender, RoutedEventArgs e)
        {
            TableView tableView = new TableView(TableType.Client);
            tableView.Show();
        }

        private void ToAttorneysButton_Click(object sender, RoutedEventArgs e)
        {
            TableView tableView = new TableView(TableType.Attorney);
            tableView.Show();
        }

        private void ToCasesButton_Click(object sender, RoutedEventArgs e)
        {
            TableView tableView = new TableView(TableType.Case);
            tableView.Show();
        }

        private void ToArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            TableView tableView = new TableView(TableType.Archive);
            tableView.Show();
        }

        private void ToArticlesButton_Click(object sender, RoutedEventArgs e)
        {
            TableView tableView = new TableView(TableType.Article);
            tableView.Show();
        }
    }
}
