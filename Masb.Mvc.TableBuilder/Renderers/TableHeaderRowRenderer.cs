using System.Collections.Generic;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a table header row rendering helper, that renders the table header cells.
    /// </summary>
    public class TableHeaderRowRenderer
    {
        private readonly IEnumerable<ITableHeaderCellRenderer> columns;

        public TableHeaderRowRenderer(IEnumerable<ITableHeaderCellRenderer> columns)
        {
            this.columns = columns;
        }

        /// <summary>
        /// Gets a renderer for each header cell.
        /// </summary>
        public IEnumerable<ITableHeaderCellRenderer> Cells
        {
            get { return this.columns; }
        }
    }
}