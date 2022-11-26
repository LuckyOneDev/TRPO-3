using System.Linq;
using System.Windows;

namespace TRPO_3
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddPerson : Window
    {
        TableType _tableType;

        public Attorney Attorney { get; }
        public Client Client { get; }

        public AddPerson(TableType type)
        {
            _tableType = type;
            InitializeComponent();
        }

        public AddPerson(Attorney attorney)
        {
            _tableType = TableType.Attorney;
            InitializeComponent();
            ConfirmButton.Content = "Изменить";
            this.Attorney = attorney;
            NameInput.Text = attorney.Human.Name;
            SurnameInput.Text = attorney.Human.Surname;
            PatronymicInput.Text = attorney.Human.Patronymic;
        }

        public AddPerson(Client client)
        {
            _tableType = TableType.Client;
            InitializeComponent();
            ConfirmButton.Content = "Изменить";
            this.Client = client;
            NameInput.Text = client.Human.Name;
            SurnameInput.Text = client.Human.Surname;
            PatronymicInput.Text = client.Human.Patronymic;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            using var context = new AttorneyContext();
            if (NameInput.Text.Length > 0
                && SurnameInput.Text.Length > 0
                && PatronymicInput.Text.Length > 0)
            {

                if (Client != null || Attorney != null)
                {
                    Human human = null;
                    switch (_tableType)
                    {
                        case TableType.Client:
                            human = context.Human.SingleOrDefault(x => x == Client.Human);
                            break;
                        case TableType.Attorney:
                            human = context.Human.SingleOrDefault(x => x == Attorney.Human);
                            break;
                    }
                    human.Name = NameInput.Text;
                    human.Surname = SurnameInput.Text;
                    human.Patronymic = PatronymicInput.Text;
                }
                else
                {
                    var human = context.Human.Add(new Human()
                    {
                        Name = NameInput.Text,
                        Surname = SurnameInput.Text,
                        Patronymic = PatronymicInput.Text,
                    });

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
                }

                context.SaveChanges();
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
