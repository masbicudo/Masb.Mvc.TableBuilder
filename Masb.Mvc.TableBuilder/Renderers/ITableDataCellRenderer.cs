using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableDataCellRenderer :
        ITemplateArgs,
        ISectionRenderer
    {
        HelperResult Render();
    }
}