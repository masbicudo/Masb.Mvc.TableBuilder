using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a column template for a row model and targeting a property of the row model.
    /// </summary>
    /// <typeparam name="TCollectionItem">Type of the of the row model.</typeparam>
    /// <typeparam name="TSubProperty">Type of the property of the row model.</typeparam>
    public class TableColumnTemplate<TCollectionItem, TSubProperty> :
        ITableColumnTemplate<TCollectionItem, TSubProperty>
    {
        private readonly Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression;
        private readonly Func<ITemplateArgs, HelperResult> header;
        private readonly Func<ITemplateArgs<TSubProperty>, HelperResult> content;
        private readonly Dictionary<string, Func<ITemplateArgs<TSubProperty>, HelperResult>> sections;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumnTemplate{TCollectionItem,TSubProperty}"/> class,
        /// with the expression to get the value from a row model,
        /// and the helpers to render the header and data cells contents,
        /// and helpers for each of the column-level sections (even if null).
        /// </summary>
        /// <param name="subPropertyExpression">
        /// Expression that retrieves a <typeparamref name="TSubProperty"/> from a <typeparamref name="TCollectionItem"/>.
        /// </param>
        /// <param name="header">
        /// Helper the renders the header contents.
        /// </param>
        /// <param name="content">
        /// Helper the renders the data cell contents.
        /// </param>
        /// <param name="sections">
        /// Helper the renders the column-level section, in the same order of the names indicated with the `SetColumnSections` method.
        /// </param>
        public TableColumnTemplate(
            Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            Func<ITemplateArgs, HelperResult> header,
            Func<ITemplateArgs<TSubProperty>, HelperResult> content,
            Dictionary<string, Func<ITemplateArgs<TSubProperty>, HelperResult>> sections)
        {
            this.subPropertyExpression = subPropertyExpression;
            this.header = header;
            this.content = content;
            this.sections = sections;
        }

        /// <summary>
        /// Gets the expression that retrieves a <typeparamref name="TSubProperty"/> from a <typeparamref name="TCollectionItem"/>.
        /// </summary>
        public Expression<Func<TCollectionItem, TSubProperty>> Expression
        {
            get { return this.subPropertyExpression; }
        }

        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableColumnTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        public TResult Accept<TResult>(ITableColumnTemplateFromVisitor<TCollectionItem, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Renders the contents of a header cell.
        /// </summary>
        /// <param name="templateArgs">Arguments that will be passed to the helper.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the header cell contents to the output stream.</returns>
        public HelperResult GetHeaderHelperResult(ITemplateArgs<TSubProperty> templateArgs)
        {
            var result = this.header(templateArgs);
            return result;
        }

        /// <summary>
        /// Renders the contents of a data cell.
        /// </summary>
        /// <param name="templateArgs">Arguments that will be passed to the helper.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the data cell contents to the output stream.</returns>
        public HelperResult GetDataHelperResult(ITemplateArgs<TSubProperty> templateArgs)
        {
            var result = this.content(templateArgs);
            return result;
        }

        /// <summary>
        /// Returns a value indicating whether the named column-level section is defined or not.
        /// </summary>
        /// <param name="sectionName">Name of the column-level section to test.</param>
        /// <returns>True if the column-level section is defined; otherwise False.</returns>
        public bool IsSectionDefined(string sectionName)
        {
            Func<ITemplateArgs<TSubProperty>, HelperResult> xpto;
            return this.sections.TryGetValue(sectionName, out xpto) && xpto != null;
        }

        /// <summary>
        /// Renders a named column-level section.
        /// </summary>
        /// <param name="sectionName">Name of the column-level section to render.</param>
        /// <param name="templateArgs">Arguments that will be passed to the column-level section when rendering.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the column-level section to the output stream.</returns>
        public HelperResult GetSectionHelperResult(string sectionName, ITemplateArgs<TSubProperty> templateArgs)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            Func<ITemplateArgs<TSubProperty>, HelperResult> sectionFunc;
            if (!this.sections.TryGetValue(sectionName, out sectionFunc) || sectionFunc == null)
                return null;

            var result = sectionFunc(templateArgs);
            return result;
        }

        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableColumnTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        public TResult Accept<TResult>(ITableColumnTemplateToVisitor<TSubProperty, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableColumnTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        public TResult Accept<TResult>(ITableColumnTemplateVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}