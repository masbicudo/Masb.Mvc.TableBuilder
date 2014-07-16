using System.Collections.Generic;

namespace Masb.Mvc.TableBuilder
{
    public class TableHeaderRowRenderer
    {
        private readonly IEnumerable<ITableHeaderCellRenderer> columns;

        public TableHeaderRowRenderer(IEnumerable<ITableHeaderCellRenderer> columns)
        {
            this.columns = columns;
        }

        public IEnumerable<ITableHeaderCellRenderer> Cells
        {
            get { return this.columns; }
        }
    }
}