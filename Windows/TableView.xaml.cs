using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TRPO_3.Windows;
using Z.EntityFramework.Plus;

namespace TRPO_3
{
    public sealed class MethodToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var methodName = parameter as string;
            if (value == null || methodName == null)
                return value;
            var methodInfo = value.GetType().GetMethod(methodName, new Type[0]);
            if (methodInfo == null)
                return value;
            return methodInfo.Invoke(value, new object[0]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }
    }

    /// <summary>
    /// Логика взаимодействия для TableView.xaml
    /// </summary>
    public partial class TableView : Window
    {
        TableType rootSchema { get; set; }
        public Attorney? SelectedAttorney { get; private set; }

        public TableView(TableType rootSchema)
        {
            InitializeComponent();
            this.rootSchema = rootSchema;
            Reload();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddButton_Click_1(object sender, RoutedEventArgs e)
        {
            switch (rootSchema)
            {
                case TableType.Attorney:
                case TableType.Client:
                    AddPerson addPersonWindow = new AddPerson(rootSchema);
                    addPersonWindow.Show();
                    addPersonWindow.Closing += (object? sender, CancelEventArgs e) =>
                    {
                        Reload();
                    };
                    break;
                case TableType.Case:
                    AddCase addCaseWindow = new AddCase(rootSchema);
                    addCaseWindow.Show();
                    addCaseWindow.Closing += (object? sender, CancelEventArgs e) =>
                    {
                        Reload();
                    };
                    break;
                case TableType.Archive:
                    break;
            }

        }

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

        private void Reload()
        {
            MainDataGrid.Items.Clear();
            MainDataGrid.IsReadOnly = true;
            using var db = new AttorneyContext();
            switch (rootSchema)
            {
                case TableType.Attorney:
                    GenerateDataGridColumns(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Id", "AttorneyId" },
                        { "Полное имя", "Human.FullName" }
                    });

                    db.Attorney.Include(x => x.Human).ToList().ForEach(x =>
                    {
                        MainDataGrid.Items.Add(x);
                    });
                    break;
                case TableType.Client:
                    InfoButton.Visibility = Visibility.Hidden;
                    GenerateDataGridColumns(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Id", "ClientId" },
                        { "Полное имя", "Human.FullName" }
                    });

                    db.Client.Include(x => x.Human).ToList().ForEach(x =>
                    {
                        MainDataGrid.Items.Add(x);
                    });

                    break;
                case TableType.Case:
                    GenerateDataGridColumns(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Клиент", "Client.Human.FullName" },
                        { "Адвокат", "Attorney.Human.FullName" },
                        { "Плата", "Pay" },
                    });

                    db.Case
                        .Include(x => x.Attorney.Human)
                        .Include(x => x.Client.Human).Where(x => x.Archived == false).ToList()
                        .ForEach(x =>
                        {
                            MainDataGrid.Items.Add(x);
                        });
                    InfoButton.Visibility = Visibility.Hidden;
                    break;
                case TableType.Archive:
                    GenerateDataGridColumns(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Клиент", "Client.Human.FullName" },
                        { "Адвокат", "Attorney.Human.FullName" },
                        { "Плата", "Pay" },
                    });

                    db.Case
                        .Include(x => x.Attorney.Human)
                        .Include(x => x.Client.Human).Where(x => x.Archived == true).ToList()
                        .ForEach(x =>
                        {
                            MainDataGrid.Items.Add(x);
                        });
                    InfoButton.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid.SelectedItem != null)
            {
                AttorneyInfo info = new AttorneyInfo(MainDataGrid.SelectedItem as Attorney);
                info.Show();
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid.SelectedItem != null)
            {
                using var db = new AttorneyContext();
                switch (rootSchema)
                {
                    case TableType.Attorney:
                        db.Attorney.RemoveRange(db.Attorney.Where(x => x == MainDataGrid.SelectedItem));
                        db.SaveChanges();
                        break;
                    case TableType.Client:
                        db.Client.RemoveRange(db.Client.Where(x => x == MainDataGrid.SelectedItem));
                        db.SaveChanges();
                        break;
                    case TableType.Case:
                    case TableType.Archive:
                        db.Case.RemoveRange(db.Case.Where(x => x == MainDataGrid.SelectedItem));
                        db.SaveChanges();
                        break;
                }
                Reload();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid.SelectedItem != null)
            {
                AddPerson addPersonWindow = null;
                switch (rootSchema)
                {
                    case TableType.Attorney:
                        addPersonWindow = new AddPerson(MainDataGrid.SelectedItem as Attorney);
                        addPersonWindow.Show();
                        addPersonWindow.Closing += (object? sender, CancelEventArgs e) =>
                        {
                            Reload();
                        };
                        break;
                    case TableType.Client:
                        addPersonWindow = new AddPerson(MainDataGrid.SelectedItem as Client);
                        addPersonWindow.Show();
                        addPersonWindow.Closing += (object? sender, CancelEventArgs e) =>
                        {
                            Reload();
                        };
                        break;
                    case TableType.Case:
                    case TableType.Archive:
                        break;
                }
            }
        }
    }
}