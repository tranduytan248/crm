using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using FastMember;

namespace TSFramework.Libs.Utils
{
    public static class UtilDataTable
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            var props =
                TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++) values[i] = props[i].GetValue(item);
                table.Rows.Add(values);
            }

            return table;
        }

        public static DataTable ToDataTable<T>(this IList<T> data, List<string> lstCols)
        {
            var completeData = new DataTable();
            using (var reader = ObjectReader.Create(data, lstCols.ToArray()))
            {
                completeData.Load(reader);
            }

            return completeData;
        }

        public static DataTable ToPivotTable<T, TColumn, TRow, TData>(
            this IEnumerable<T> source,
            Func<T, TColumn> columnSelector,
            Expression<Func<T, TRow>> rowSelector,
            Func<IEnumerable<T>, TData> dataSelector)
        {
            var table = new DataTable();
            var rowName = ((MemberExpression)rowSelector.Body).Member.Name;
            table.Columns.Add(new DataColumn(rowName));
            var columns = source.Select(columnSelector).Distinct();

            foreach (var column in columns)
                table.Columns.Add(new DataColumn(column.ToString()));

            var rows = source.GroupBy(rowSelector.Compile())
                .Select(rowGroup => new
                {
                    rowGroup.Key,
                    Values = columns.GroupJoin(
                        rowGroup,
                        c => c,
                        r => columnSelector(r),
                        (c, columnGroup) => dataSelector(columnGroup))
                });

            foreach (var row in rows)
            {
                var dataRow = table.NewRow();
                var items = row.Values.Cast<object>().ToList();
                items.Insert(0, row.Key);
                dataRow.ItemArray = items.ToArray();
                table.Rows.Add(dataRow);
            }

            return table;
        }
    }
}
