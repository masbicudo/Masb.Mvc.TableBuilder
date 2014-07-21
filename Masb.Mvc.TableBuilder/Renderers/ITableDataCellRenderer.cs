using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents the ability to render table cell contents.
    /// </summary>
    public interface ITableDataCellRenderer :
        IHelperContext,
        ISectionRenderer
    {
        /// <summary>
        /// Renders the current data cell contents.
        /// </summary>
        /// <returns>A <see cref="HelperResult"/> that writes the contents of the cell to the output stream.</returns>
        HelperResult Render();
    }
}