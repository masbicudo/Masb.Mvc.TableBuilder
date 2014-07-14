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
        private readonly int indexToRender;
        private readonly string indexHiddenFieldName;
        private readonly string indexHiddenElementId;

        public TableDataRowRenderer(
            IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> columns,
            HtmlHelper<TCollectionItem> html,
            int indexToRender,
            string indexHiddenFieldName,
            string indexHiddenElementId)
        {
            this.columns = columns;
            this.html = html;
            this.indexToRender = indexToRender;
            this.indexHiddenFieldName = indexHiddenFieldName;
            this.indexHiddenElementId = indexHiddenElementId;
        }

        public IEnumerable<ITableDataCellRenderer> Cells
        {
            get
            {
                var result = this.columns.Select(col => new TableDataCellRenderer<TCollectionItem>(col, this.html));
                return result;
            }
        }

        public MvcHtmlString RenderIndexHiddenField()
        {
            return
                new MvcHtmlString(
                    string.Format(
                        @"<input type=""hidden"" id=""{0}"" name=""{1}"" value""{2}"" />",
                        this.indexHiddenElementId,
                        this.indexHiddenFieldName,
                        this.indexToRender));
        }

        public MvcHtmlString RenderIndexHiddenField(string @class)
        {
            return
                new MvcHtmlString(
                    string.Format(
                        @"<input type=""hidden"" id=""{0}"" name=""{1}"" value""{2}"" class=""{3}"" />",
                        this.indexHiddenElementId,
                        this.indexHiddenFieldName,
                        this.indexToRender,
                        @class));
        }
    }
}