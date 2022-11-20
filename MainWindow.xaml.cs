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
        }

        private void ToClientsButton_Click(object sender, RoutedEventArgs e)
        {
            TableView clientsWindow = new TableView(TableType.Client);
            clientsWindow.Show();
        }

        private void ToAttorneysButton_Click(object sender, RoutedEventArgs e)
        {
            TableView attorneyWindow = new TableView(TableType.Attorney);
            attorneyWindow.Show();
        }

        private void ToCasesButton_Click(object sender, RoutedEventArgs e)
        {
            TableView caseWindow = new TableView(TableType.Case);
            caseWindow.Show();
        }
    }
}
