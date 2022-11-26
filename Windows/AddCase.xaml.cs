using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TRPO_3.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddCase.xaml
    /// </summary>
    public partial class AddCase : Window
    {
        string descriptionText = "";
        List<Attorney> attorneys;
        List<Client> clients;
        TableType _tableType;

        public AddCase(TableType type)
        {
            InitializeComponent();
            _tableType = type;
            using var db = new AttorneyContext();
            attorneys = db.Attorney.ToList();
            clients = db.Client.ToList();
            AttorneyInput.ItemsSource = db.Attorney.Select(x => x.Human).ToList().Select(x => x.FullName);
            ClientInput.ItemsSource = db.Client.Select(x => x.Human).ToList().Select(x => x.FullName);
            EndDateInput.Text = DateTime.Now.ToLongDateString();
        }

        private void PayInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(PayInput.Text, out _))
            {
                PayInput.Clear();
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            using var context = new AttorneyContext();
            try
            {
                var attorney = context.Attorney.Attach(attorneys.Find(x => x.Human.FullName == AttorneyInput.Text));
                var client = context.Client.Attach(clients.Find(x => x.Human.FullName == ClientInput.Text));
                var myCase = new Case(
                    attorney.Entity,
                    client.Entity,
                    int.Parse(PayInput.Text),
                    StartDateInput.Text,
                    EndDateInput.Text,
                    new TextRange(DescriptionInput.Document.ContentStart, DescriptionInput.Document.ContentEnd).Text
                );

                context.Case.Add(myCase);
                context.SaveChanges();

            }
            catch (Exception ex)
            {

            }

            this.Close();

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DescriptionInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            //descriptionText = e.
        }
    }
}
