using System.Collections.Generic;

namespace Masb.Mvc.TableBuilder.Code
{
    public class TableHeaderRowRenderer
    {
        private readonly IEnumerable<TableHeaderCellRenderer> columns;

        public TableHeaderRowRenderer(IEnumerable<TableHeaderCellRenderer> columns)
        {
            this.columns = columns;
        }

        public IEnumerable<TableHeaderCellRenderer> Cells
        {
            get { return this.columns; }
        }
    }
}