using System.Collections.Generic;
using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableDataRowRenderer :
        ITemplateArgs,
        ISectionRenderer
    {
        /// <summary>
        /// Gets the cells that compose this data row.
        /// </summary>
        IEnumerable<ITableDataCellRenderer> Cells { get; }

        /// <summary>
        /// Renders the hidden field responsible for indicating the existence of this index inside the collection,
        /// through the ".Index" property.
        /// </summary>
        /// <returns>An <see cref="MvcHtmlString"/> with the hidden field to render.</returns>
        MvcHtmlString RenderIndexHiddenField();

        /// <summary>
        /// Renders the hidden field responsible for indicating the existence of this index inside the collection,
        /// through the ".Index" property.
        /// </summary>
        /// <param name="class"> The CSS class name that will be rendered for the hidden input field. </param>
        /// <returns> An <see cref="MvcHtmlString"/> with the hidden field to render. </returns>
        MvcHtmlString RenderIndexHiddenField(string @class);
    }
}