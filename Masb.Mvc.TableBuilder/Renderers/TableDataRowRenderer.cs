using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder
{
    public class TableDataRowRenderer<TCollectionItem> :
        ITableDataRowRenderer
    {
        private readonly IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> columns;
        private readonly HtmlHelper<TCollectionItem> html;

        public TableDataRowRenderer(IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> columns, HtmlHelper<TCollectionItem> html)
        {
            this.columns = columns;
            this.html = html;
        }

        public IEnumerable<ITableDataCellRenderer> Cells
        {
            get
            {
                var result = this.columns.Select(col => new TableDataCellRenderer<TCollectionItem>(col, this.html));
                return result;
            }
        }
    }
}