using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddArticle.xaml
    /// </summary>
    public partial class AddArticle : Window
    {
        public AddArticle()
        {
            InitializeComponent();
            ExprectedPunishmentInput.PreviewTextInput += ExprectedPunishmentInput_PreviewTextInput;
        }

        private void ExprectedPunishmentInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var db = new AttorneyContext();
            db.Article.Add(new Article()
            {
                Name = ArticleNameInput.Text,
                ExprectedPunishment = int.Parse(ExprectedPunishmentInput.Text)
            });

            db.SaveChanges();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
    }
}
