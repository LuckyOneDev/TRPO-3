using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TRPO_3.Windows;
using Z.EntityFramework.Plus;

namespace TRPO_3
{
    /// <summary>
    /// Логика взаимодействия для ClientsWindow.xaml
    /// </summary>
    public partial class TableView : Window
    {
        TableType rootSchema { get; set; }
        public TableView(TableType rootSchema)
        {
            InitializeComponent();
            this.rootSchema = rootSchema;
            Reload();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using var db = new AttorneyContext();
            foreach (var item in e.RemovedItems)
            {
                if (item is Human || item is Case)
                {
                    switch (rootSchema)
                    {
                        case TableType.Attorney:
                            db.Attorney.RemoveRange(db.Attorney.Where(x => x.Human == ((Human)item)));
                            db.SaveChanges();
                            break;
                        case TableType.Client:
                            db.Client.RemoveRange(db.Client.Where(x => x.Human == ((Human)item)));
                            db.SaveChanges();
                            break;
                        case TableType.Case:
                            db.Case.Remove((Case)item);
                            db.SaveChanges();
                            break;
                    }
                }
            }
        }

        private void AddButton_Click_1(object sender, RoutedEventArgs e)
        {
            switch (rootSchema)
            {
                case TableType.Attorney:
                case TableType.Client:
                    AddPerson addPersonWindow = new AddPerson(rootSchema);
                    addPersonWindow.Show();
                    break;
                case TableType.Case:
                    AddCase addCaseWindow = new AddCase(rootSchema);
                    addCaseWindow.Show();
                    break;
            }

        }

        private void Reload()
        {
            using var db = new AttorneyContext();
            switch (rootSchema)
            {
                case TableType.Attorney:
                    MainDataGrid.ItemsSource = db.Attorney.Select(x => new
                    {
                        Id = x.AttorneyId,
                        Name = x.Human.FullName
                    }).ToList();

                    break;
                case TableType.Case:
                    MainDataGrid.ItemsSource = db.Case.Select(x => new
                    {
                        ID = x.CaseId,
                        Attorney = x.Attorney.Human.FullName,
                        Client = x.Client.Human.FullName,
                        Pay = x.Pay,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate

                    }).ToList();
                    break;
                case TableType.Client:
                    MainDataGrid.ItemsSource = db.Client.Select(x => new
                    {
                        Id = x.ClientId,
                        Name = x.Human.FullName
                    }).ToList();
                    break;
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            Reload();
        }
    }
}
