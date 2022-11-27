using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
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
        private TableType _tableType;

        public AddCase(TableType type)
        {
            InitializeComponent();
            _tableType = type;
            using var db = new AttorneyContext();

            AttorneyInput.DisplayMemberPath = "Human.FullName";
            ClientInput.DisplayMemberPath = "Human.FullName";
            ArticleInput.DisplayMemberPath = "Name";

            AttorneyInput.ItemsSource = db.Attorney.Include(x => x.Human).ToList();
            ClientInput.ItemsSource = db.Client.Include(x => x.Human).ToList();
            ArticleInput.ItemsSource = db.Article.ToList();

            StartDateInput.Text = DateTime.Now.ToLongDateString();

            PayInput.PreviewTextInput += PayInput_PreviewTextInput;
        }

        private void PayInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AttorneyInput.SelectedValue != null
                && ClientInput.SelectedValue != null
                && ArticleInput.SelectedValue != null
                && int.TryParse(PayInput.Text, out _)
                && DateTime.TryParse(StartDateInput.Text, out _)
                )
            {
                using var context = new AttorneyContext();
                var myCase = new Case()
                {
                    Attorney = context.Attach(AttorneyInput.SelectedValue as Attorney).Entity,
                    Client = context.Attach(ClientInput.SelectedValue as Client).Entity,
                    Article = context.Attach(ArticleInput.SelectedValue as Article).Entity,
                    Pay = int.Parse(PayInput.Text),
                    StartDate = DateTime.Parse(StartDateInput.Text),
                    Comment = new TextRange(DescriptionInput.Document.ContentStart, DescriptionInput.Document.ContentEnd).Text,
                    Archived = false
                };

                context.Case.Add(myCase);
                context.SaveChanges();

                this.Close();
            }
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