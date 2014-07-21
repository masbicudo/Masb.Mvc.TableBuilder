using System.Collections.Generic;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a table template targeting a known items collection.
    /// </summary>
    /// <typeparam name="TCollectionItem">Type of the items in the target items collection.</typeparam>
    public interface ITableTemplateTo<TCollectionItem> :
        ITableTemplate
    {
        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        TResult Accept<TResult>(ITableTemplateToVisitor<TCollectionItem, TResult> visitor);

        /// <summary>
        /// Gets the columns defined by this table template.
        /// </summary>
        new IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> Columns { get; }

        /// <summary>
        /// Returns a value indicating whether the named table-level section is defined or not.
        /// </summary>
        /// <param name="sectionName">Name of the table-level section to test.</param>
        /// <param name="templateArgs">Arguments that will be passed to the table-level section when rendering.</param>
        /// <returns>True if the table-level section is defined; otherwise False.</returns>
        bool IsSectionDefined([NotNull] string sectionName, ITemplateArgs<IList<TCollectionItem>> templateArgs);

        /// <summary>
        /// Renders a named table-level section.
        /// </summary>
        /// <param name="sectionName">Name of the table-level section to render.</param>
        /// <param name="templateArgs">Arguments that will be passed to the table-level section when rendering.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the table-level section to the output stream.</returns>
        [CanBeNull]
        HelperResult GetSectionHelperResult([NotNull] string sectionName, ITemplateArgs<IList<TCollectionItem>> templateArgs);

        /// <summary>
        /// Returns a value indicating whether the named item-level section is defined or not.
        /// </summary>
        /// <param name="sectionName">Name of the item-level section to test.</param>
        /// <param name="templateArgs">Arguments that will be passed to the item-level section when rendering.</param>
        /// <returns>True if the item-level section is defined; otherwise False.</returns>
        bool IsItemSectionDefined([NotNull] string sectionName, ITemplateArgsWithData<TCollectionItem, RowInfo> templateArgs);

        /// <summary>
        /// Renders a named item-level section.
        /// </summary>
        /// <param name="sectionName">Name of the item-level section to render.</param>
        /// <param name="templateArgs">Arguments that will be passed to the item-level section when rendering.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the item-level section to the output stream.</returns>
        [CanBeNull]
        HelperResult GetItemSectionHelperResult([NotNull] string sectionName, ITemplateArgsWithData<TCollectionItem, RowInfo> templateArgs);
    }
}