using System.Windows;

namespace TRPO_3
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddPerson : Window
    {
        TableType _tableType;
        public AddPerson(TableType type)
        {
            _tableType = type;
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            using var context = new AttorneyContext();
            var human = context.Human.Add(new Human()
            {
                Name = NameInput.Text,
                Surname = SurnameInput.Text,
                Patronymic = PatronymicInput.Text,
            });

            context.SaveChanges();

            switch (_tableType)
            {
                case TableType.Client:
                    var client = new Client(human.Entity);
                    context.Client.Add(client);
                    break;
                case TableType.Attorney:
                    var attorney = new Attorney(human.Entity);
                    context.Attorney.Add(attorney);
                    break;
            }
            context.SaveChanges();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
