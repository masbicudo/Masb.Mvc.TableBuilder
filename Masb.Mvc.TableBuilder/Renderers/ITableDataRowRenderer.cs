using System.Collections.Generic;
using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableDataRowRenderer
    {
        IEnumerable<ITableDataCellRenderer> Cells { get; }

        MvcHtmlString RenderIndexHiddenField();

        MvcHtmlString RenderIndexHiddenField(string @class);
    }
}