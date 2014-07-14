using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Masb.Mvc.TableBuilder;

namespace Masb.Mvc.TableBuilder
{
    public class TableDataRowRenderer<TCollectionItem>
    {
        private readonly IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> columns;
        private readonly HtmlHelper<TCollectionItem> html;

        public TableDataRowRenderer(IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> columns, HtmlHelper<TCollectionItem> html)
        {
            this.columns = columns;
            this.html = html;
        }

        public IEnumerable<TableDataCellRenderer<TCollectionItem>> Cells
        {
            get
            {
                var result = this.columns.Select(col => new TableDataCellRenderer<TCollectionItem>(col, this.html));
                return result;
            }
        }
    }
}