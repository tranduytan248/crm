using System.Collections.Generic;
using TSFramework.Libs.Attributes;

namespace TSFramework.Libs.Models.Base
{
    public class BaseSearchModel
    {
        public string Search { get; set; }

        /// <summary>
        ///     Sort Column
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        ///     Sort Type : DESC | ASC
        /// </summary>
        public string OrderDir { get; set; }

        /// <summary>
        ///     Current Row Index
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        ///     Page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Output: total rows
        /// </summary>
        public int TotalRow { get; set; }
    }

    public class SearchModel
    {
        [Alias("draw")]
        public int Draw { get; set; }

        [Alias("columns")]
        public List<Column> Columns { get; set; }

        [Alias("order")]
        public List<Order> Order { get; set; }

        [Alias("start")]
        public int Start { get; set; }

        [Alias("length")]
        public int Length { get; set; }

        [Alias("search")]
        public Search Search { get; set; }
    }

    public class Column
    {
        [Alias("data")]
        public string Data { get; set; }

        [Alias("name")]
        public string Name { get; set; }

        [Alias("searchable")]
        public bool Searchable { get; set; }

        [Alias("orderable")]
        public bool Orderable { get; set; }

        [Alias("search")]
        public Search Search { get; set; }
    }

    public class Search
    {
        [Alias("value")]
        public string Value { get; set; }

        [Alias("regex")]
        public bool Regex { get; set; }
    }

    public class Order
    {
        [Alias("column")]
        public string Column { get; set; }

        [Alias("dir")]
        public string Dir { get; set; }
    }
}