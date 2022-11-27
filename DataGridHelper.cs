using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace TRPO_3
{
    public static class DataGridHelper
    {
        public static void FillDataGrid<T>(DataGrid dg, Dictionary<string, string> map, IEnumerable<T> query)
        {
            GenerateDataGridColumns(dg, map);
            AddItems(dg, query);
        }
        private static void AddItems<T>(DataGrid dg, IEnumerable<T> query)
        {
            query.ToList().ForEach(x =>
            {
                dg.Items.Add(x);
            });
        }
        private static void GenerateDataGridColumns(DataGrid dg, Dictionary<string, string> map)
        {
            dg.AutoGenerateColumns = false;
            dg.Columns.Clear();
            dg.Items.Clear();
            foreach (var item in map)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = item.Key;
                textColumn.Binding = new Binding(item.Value);
                dg.Columns.Add(textColumn);
            }
        }
    }
}
