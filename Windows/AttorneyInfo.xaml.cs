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
        public Attorney Attorney { get; private set; }
        void GenerateCaseContents(Attorney attorney, bool archived = true)
        {
            using var db = new AttorneyContext();
            CaseDataGrid.IsReadOnly = true;
            DataGridHelper.FillDataGrid(CaseDataGrid, new Dictionary<string, string>()
            {
                { "Id", "CaseId" },
                { "Клиент", "Client.Human.FullName" },
                { "Плата", "Pay" },
                { "Срок", "FinalPunishment" },
            }, db.Case.Where(x => x.Attorney == attorney && x.Archived == archived).Include(x => x.Client.Human));
        }

        void GenerateClientContents(Attorney attorney, bool archived = true)
        {
            using var db = new AttorneyContext();
            ClientDataGrid.IsReadOnly = true;
            DataGridHelper.FillDataGrid(ClientDataGrid, new Dictionary<string, string>()
            {
                { "Id", "ClientId" },
                { "Имя клиента", "Client.Human.FullName" }
            }, db.Case.Where(x => x.Attorney == attorney && x.Archived == archived).Include(x => x.Client.Human));
        }

        public AttorneyInfo(Attorney attorney)
        {
            this.Attorney = attorney;
            InitializeComponent();
            AttorneyName.Content = attorney.Human.FullName;

            using var db = new AttorneyContext();
            PayLabel.Content = db.Case
                .Where(x => x.Attorney == attorney && x.Archived == true)
                .Sum(x => x.Pay) + " рублей";

            ClientCheck.Click += ClientCheck_Click;
            CaseCheck.Click += CaseCheck_Click;

            GenerateClientContents(attorney);
            GenerateCaseContents(attorney);
        }

        private void CaseCheck_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox check)
            {
                GenerateCaseContents(Attorney, check.IsChecked.Value);
            }
        }

        private void ClientCheck_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox check)
            {
                GenerateClientContents(Attorney, check.IsChecked.Value);
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
