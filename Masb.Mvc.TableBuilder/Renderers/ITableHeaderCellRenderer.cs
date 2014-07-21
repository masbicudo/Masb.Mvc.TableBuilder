using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents the ability to render table header cell contents.
    /// </summary>
    public interface ITableHeaderCellRenderer :
        IHelperContext,
        ISectionRenderer
    {
        /// <summary>
        /// Renders the current header cell contents.
        /// </summary>
        /// <returns>A <see cref="HelperResult"/> that writes the contents of the header cell to the output stream.</returns>
        HelperResult Render();
    }
}