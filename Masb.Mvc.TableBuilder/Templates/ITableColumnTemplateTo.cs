using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a column template targeting a know property of the row model.
    /// </summary>
    /// <typeparam name="TSubProperty">Type of the property of the row model.</typeparam>
    public interface ITableColumnTemplateTo<TSubProperty> :
        ITableColumnTemplate
    {
        /// <summary>
        /// Renders the contents of a header cell.
        /// </summary>
        /// <param name="templateArgs">Arguments that will be passed to the helper.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the header cell contents to the output stream.</returns>
        [NotNull]
        HelperResult GetHeaderHelperResult([NotNull] ITemplateArgs<TSubProperty> templateArgs);

        /// <summary>
        /// Renders the contents of a data cell.
        /// </summary>
        /// <param name="templateArgs">Arguments that will be passed to the helper.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the data cell contents to the output stream.</returns>
        [NotNull]
        HelperResult GetDataHelperResult([NotNull] ITemplateArgs<TSubProperty> templateArgs);

        /// <summary>
        /// Returns a value indicating whether the named column-level section is defined or not.
        /// </summary>
        /// <param name="sectionName">Name of the column-level section to test.</param>
        /// <returns>True if the column-level section is defined; otherwise False.</returns>
        bool IsSectionDefined([NotNull] string sectionName);

        /// <summary>
        /// Renders a named column-level section.
        /// </summary>
        /// <param name="sectionName">Name of the column-level section to render.</param>
        /// <param name="templateArgs">Arguments that will be passed to the column-level section when rendering.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the column-level section to the output stream.</returns>
        [CanBeNull]
        HelperResult GetSectionHelperResult([NotNull] string sectionName, [NotNull] ITemplateArgs<TSubProperty> templateArgs);

        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableColumnTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        TResult Accept<TResult>([NotNull] ITableColumnTemplateToVisitor<TSubProperty, TResult> visitor);
    }
}