using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TRPO_3.Windows
{
    /// <summary>
    /// Interaction logic for CloseCase.xaml
    /// </summary>
    public partial class CloseCase : Window
    {
        Case Case { get; set; }
        public CloseCase(Case c)
        {
            InitializeComponent();
            Case = c;
            AttorneyName.Content = c.Attorney.Human.FullName;
            ClientName.Content = c.Client.Human.FullName;
            ArticleName.Content = c.Article.Name;
            PayInput.Text = c.Pay.ToString();
            StartDateInput.Content = c.StartDate.ToLongDateString();
            EndDateInput.SelectedDate = DateTime.Now;
            DescriptionInput.AppendText(c.Comment);
            PunishMentInput.Text = c.Article.ExprectedPunishment.ToString();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var db = new AttorneyContext();
            db.Case.Attach(Case);
            Case.Pay = int.Parse(PayInput.Text);
            Case.Comment = new TextRange(DescriptionInput.Document.ContentStart, DescriptionInput.Document.ContentEnd).Text;
            Case.EndDate = EndDateInput.DisplayDate;
            Case.FinalPunishment = int.Parse(PunishMentInput.Text);
            Case.Archived = true;
            db.SaveChanges();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
