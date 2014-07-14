using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableDataCellRenderer :
        IViewTemplate
    {
        HelperResult Render();
    }
}