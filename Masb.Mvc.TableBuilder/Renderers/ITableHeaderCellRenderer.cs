using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableHeaderCellRenderer :
        IViewTemplate,
        ISectionRenderer
    {
        HelperResult Render();
    }
}