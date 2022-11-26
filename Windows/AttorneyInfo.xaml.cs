using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TRPO_3.Windows
{
    /// <summary>
    /// Логика взаимодействия для AttorneyInfo.xaml
    /// </summary>
    public partial class AttorneyInfo : Window
    {
        private void GenerateDataGridColumns(DataGrid dg, Dictionary<string, string> map)
        {
            MainDataGrid.AutoGenerateColumns = false;
            dg.Columns.Clear();
            foreach (var item in map)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = item.Key;
                textColumn.Binding = new Binding(item.Value);
                dg.Columns.Add(textColumn);
            }
        }

        public AttorneyInfo(Attorney attorney)
        {
            InitializeComponent();
            AttorneyName.Content = attorney.Human.FullName;
            using var db = new AttorneyContext();

            GenerateDataGridColumns(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Id", "ClientId" },
                        { "Имя клиента", "Client.Human.FullName" }
                    });
            db.Case
                .Where(x => x.Attorney == attorney)
                .Include(x => x.Client.Human).ToList()
                .ForEach(x =>
                {
                    MainDataGrid.Items.Add(x);
                });

            PayLabel.Content = db.Case
                .Where(x => x.Attorney == attorney)
                .Sum(x => x.Pay) + " рублей";
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
