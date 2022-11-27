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
                case TableType.Article:
                    AddArticle addArticleWindow = new AddArticle();
                    addArticleWindow.Show();
                    addArticleWindow.Closing += (object? sender, CancelEventArgs e) =>
                    {
                        Reload();
                    };
                    break;
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
                    DataGridHelper.FillDataGrid(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Id", "AttorneyId" },
                        { "Полное имя", "Human.FullName" }
                    }, db.Attorney.Include(x => x.Human));
                    break;
                case TableType.Client:
                    InfoButton.Visibility = Visibility.Hidden;
                    DataGridHelper.FillDataGrid(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Id", "ClientId" },
                        { "Полное имя", "Human.FullName" }
                    }, db.Client.Include(x => x.Human));
                    break;
                case TableType.Case:
                    InfoButton.Content = "Закрыть дело";
                    DataGridHelper.FillDataGrid(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Клиент", "Client.Human.FullName" },
                        { "Адвокат", "Attorney.Human.FullName" },
                        { "Плата", "Pay" },
                        { "Статья", "Article.Name" },
                        { "Ожидаемый срок", "Article.ExprectedPunishment" }
                    }, db.Case.Include(x => x.Attorney.Human).Include(x => x.Client.Human).Include(x => x.Article).Where(x => x.Archived == false));
                    break;
                case TableType.Archive:
                    InfoButton.Visibility = Visibility.Hidden;
                    AddButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    DataGridHelper.FillDataGrid(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Клиент", "Client.Human.FullName" },
                        { "Адвокат", "Attorney.Human.FullName" },
                        { "Плата", "Pay" },
                        { "Статья", "Article.Name" },
                        { "Ожидаемый срок", "Article.ExprectedPunishment" },
                        { "Итоговый срок", "FinalPunishment" }
                    }, db.Case.Include(x => x.Attorney.Human).Include(x => x.Client.Human).Include(x => x.Article).Where(x => x.Archived == true));
                    break;
                case TableType.Article:
                    InfoButton.Visibility = Visibility.Hidden;
                    EditButton.IsEnabled = false;
                    DataGridHelper.FillDataGrid(MainDataGrid, new Dictionary<string, string>()
                    {
                        { "Id", "ArticleId" },
                        { "Название", "Name" },
                        { "Ожидаемый срок", "ExprectedPunishment" }
                    }, db.Article);
                    break;
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            switch (rootSchema)
            {
                case TableType.Attorney:
                    if (MainDataGrid.SelectedItem != null)
                    {
                        AttorneyInfo info = new AttorneyInfo(MainDataGrid.SelectedItem as Attorney);
                        info.Show();
                    }
                    break;
                case TableType.Case:
                    if (MainDataGrid.SelectedItem != null)
                    {
                        CloseCase cc = new CloseCase(MainDataGrid.SelectedItem as Case);
                        cc.Show();
                        cc.Closing += (object? sender, CancelEventArgs e) =>
                        {
                            Reload();
                        };
                    }
                    break;
                default:
                    break;
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
                    case TableType.Article:
                        db.Article.RemoveRange(db.Article.Where(x => x == MainDataGrid.SelectedItem));
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