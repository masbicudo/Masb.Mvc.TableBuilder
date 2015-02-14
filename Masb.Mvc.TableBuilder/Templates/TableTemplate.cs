using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a table template for a root model and associated collection of items.
    /// </summary>
    /// <typeparam name="TModel">Type of the root model that contains the items collection.</typeparam>
    /// <typeparam name="TCollectionItem">Type of the items in the target items collection.</typeparam>
    public class TableTemplate<TModel, TCollectionItem> :
        ITableTemplate<TModel, TCollectionItem>
    {
        private readonly Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression;

        private readonly List<ITableColumnTemplateFrom<TCollectionItem>> columns
            = new List<ITableColumnTemplateFrom<TCollectionItem>>();

        private readonly Dictionary<string, ITemplateSection<ITemplateArgs<IList<TCollectionItem>>, HelperResult>> rootSections
            = new Dictionary<string, ITemplateSection<ITemplateArgs<IList<TCollectionItem>>, HelperResult>>();

        private readonly Dictionary<string, ITemplateSection<ITemplateArgsWithData<TCollectionItem, RowInfo>, HelperResult>> itemSections
            = new Dictionary<string, ITemplateSection<ITemplateArgsWithData<TCollectionItem, RowInfo>, HelperResult>>();

        [CanBeNull]
        private string[] columnSectioNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableTemplate{TModel,TCollectionItem}"/> class,
        /// given an expression that gets the list of <typeparamref name="TCollectionItem"/> from a given <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="collectionExpression">
        /// Expression that gets the list of <typeparamref name="TCollectionItem"/> from a given <typeparamref name="TModel"/>.
        /// </param>
        public TableTemplate(Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression)
        {
            this.collectionExpression = collectionExpression;
        }

        /// <summary>
        /// Adds a column to the table template, targeting a specific item property.
        /// </summary>
        /// <param name="subPropertyExpression">Expression that gets the property from the item.</param>
        /// <param name="header">Header helper returning a HelperResult given the contextual template arguments.</param>
        /// <param name="content">Content helper to render data cells, returning a HelperResult given the contextual template arguments.</param>
        /// <param name="sections">Column-level section helpers returning a HelperResult given the contextual template arguments.</param>
        /// <typeparam name="TSubProperty">Type of the property of the item.</typeparam>
        /// <returns>The current <see cref="TableTemplate&lt;TModel, TCollectionItem>"/> for fluent configuration.</returns>
        public TableTemplate<TModel, TCollectionItem> AddColumnFor<TSubProperty>(
            Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            Func<ITemplateArgs, HelperResult> header,
            Func<ITemplateArgs<TSubProperty>, HelperResult> content,
            params Func<ITemplateArgs<TSubProperty>, HelperResult>[] sections)
        {
            var sectionsDic = GetSectionsDictionary(sections);
            this.columns.Add(new TableColumnTemplate<TCollectionItem, TSubProperty>(subPropertyExpression, header, content, sectionsDic));
            return this;
        }

        /// <summary>
        /// Adds a column to the table template.
        /// </summary>
        /// <param name="header">Header helper returning a HelperResult given the contextual template arguments.</param>
        /// <param name="content">Content helper to render data cells, returning a HelperResult given the contextual template arguments.</param>
        /// <param name="sections">Column-level section helpers returning a HelperResult given the contextual template arguments.</param>
        /// <returns>The current <see cref="TableTemplate&lt;TModel, TCollectionItem>"/> for fluent configuration.</returns>
        public TableTemplate<TModel, TCollectionItem> AddColumn(
            Func<ITemplateArgs, HelperResult> header,
            Func<ITemplateArgs<TCollectionItem>, HelperResult> content,
            params Func<ITemplateArgs<TCollectionItem>, HelperResult>[] sections)
        {
            var sectionsDic = GetSectionsDictionary(sections);
            this.columns.Add(new TableColumnTemplate<TCollectionItem, TCollectionItem>(x => x, header, content, sectionsDic));
            return this;
        }

        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        public TResult Accept<TResult>(ITableTemplateVisitor<TModel, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        public TResult Accept<TResult>(ITableTemplateToVisitor<TCollectionItem, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Gets the columns defined by this table template.
        /// </summary>
        public IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> Columns
        {
            get { return new ReadOnlyCollection<ITableColumnTemplateFrom<TCollectionItem>>(this.columns); }
        }

        /// <summary>
        /// Returns a value indicating whether the named table-level section is defined or not.
        /// </summary>
        /// <param name="sectionName">Name of the table-level section to test.</param>
        /// <param name="templateArgs">Arguments that will be passed to the table-level section when rendering.</param>
        /// <returns>True if the table-level section is defined; otherwise False.</returns>
        public bool IsSectionDefined(string sectionName, ITemplateArgs<IList<TCollectionItem>> templateArgs)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            ITemplateSection<ITemplateArgs<IList<TCollectionItem>>, HelperResult> xpto;
            var result = this.rootSections.TryGetValue(sectionName, out xpto) && xpto != null && xpto.CanRender(templateArgs);
            return result;
        }

        /// <summary>
        /// Renders a named table-level section.
        /// </summary>
        /// <param name="sectionName">Name of the table-level section to render.</param>
        /// <param name="templateArgs">Arguments that will be passed to the table-level section when rendering.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the table-level section to the output stream.</returns>
        public HelperResult GetSectionHelperResult(
            string sectionName,
            ITemplateArgs<IList<TCollectionItem>> templateArgs)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            ITemplateSection<ITemplateArgs<IList<TCollectionItem>>, HelperResult> xpto;
            if (!this.rootSections.TryGetValue(sectionName, out xpto) || xpto == null || !xpto.CanRender(templateArgs))
                return null;

            var result = xpto.Render(templateArgs);
            return result;
        }

        /// <summary>
        /// Returns a value indicating whether the named item-level section is defined or not.
        /// </summary>
        /// <param name="sectionName">Name of the item-level section to test.</param>
        /// <param name="templateArgs">Arguments that will be passed to the item-level section when rendering.</param>
        /// <returns>True if the item-level section is defined; otherwise False.</returns>
        public bool IsItemSectionDefined(
            string sectionName,
            ITemplateArgsWithData<TCollectionItem, RowInfo> templateArgs)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            ITemplateSection<ITemplateArgsWithData<TCollectionItem, RowInfo>, HelperResult> xpto;
            var result = this.itemSections.TryGetValue(sectionName, out xpto) && xpto != null
                         && xpto.CanRender(templateArgs);
            return result;
        }

        /// <summary>
        /// Renders a named item-level section.
        /// </summary>
        /// <param name="sectionName">Name of the item-level section to render.</param>
        /// <param name="templateArgs">Arguments that will be passed to the item-level section when rendering.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the item-level section to the output stream.</returns>
        public HelperResult GetItemSectionHelperResult(
            string sectionName,
            ITemplateArgsWithData<TCollectionItem, RowInfo> templateArgs)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            ITemplateSection<ITemplateArgsWithData<TCollectionItem, RowInfo>, HelperResult> xpto;
            if (!this.itemSections.TryGetValue(sectionName, out xpto) || xpto == null || !xpto.CanRender(templateArgs))
                return null;

            var result = xpto.Render(templateArgs);
            return result;
        }

        /// <summary>
        /// Gets the columns defined by this table template.
        /// </summary>
        IEnumerable<ITableColumnTemplate> ITableTemplate.Columns
        {
            get { return this.Columns; }
        }

        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        public TResult Accept<TResult>(ITableTemplateVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Gets the expression that retrieves a list of <typeparamref name="TCollectionItem"/> from a <typeparamref name="TModel"/>.
        /// </summary>
        public Expression<Func<TModel, IList<TCollectionItem>>> Expression
        {
            get { return this.collectionExpression; }
        }

        /// <summary>
        /// Gets the names of the defined column sections.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<string> ColumnSectioNames
        {
            get
            {
                return this.columnSectioNames == null || this.columnSectioNames.Length == 0
                    ? Enumerable.Empty<string>()
                    : new ReadOnlyCollection<string>(this.columnSectioNames);
            }
        }

        /// <summary>
        /// Sets the list of named column-level sections.
        /// All of these section must be indicated when adding columns to this table template.
        /// </summary>
        /// <param name="sectioNames">Names of the column-level sections.</param>
        /// <returns>The current <see cref="TableTemplate&lt;TModel, TCollectionItem>"/> for fluent configuration.</returns>
        public TableTemplate<TModel, TCollectionItem> SetColumnSections([NotNull] params string[] sectioNames)
        {
            if (sectioNames == null)
                throw new ArgumentNullException("sectioNames");

            if (this.columns.Count > 0)
                throw new Exception("Cannot set column sections after adding columns.");

            if (this.columnSectioNames != null)
                throw new Exception("Cannot set column sections more than once.");

            if (sectioNames.Distinct().Count() != sectioNames.Length)
                throw new Exception("Section names cannot be repeated.");

            this.columnSectioNames = sectioNames;

            return this;
        }

        private Dictionary<string, Func<ITemplateArgs<TSubProperty>, HelperResult>> GetSectionsDictionary<TSubProperty>(Func<ITemplateArgs<TSubProperty>, HelperResult>[] sections)
        {
            var definedSections = this.columnSectioNames ?? new string[0];
            var passedSections = sections ?? new Func<ITemplateArgs<TSubProperty>, HelperResult>[0];
            if (definedSections.Length != passedSections.Length)
                throw new Exception("Number of defined sections is different from the number of passed sections.");

            var sectionsDic = new Dictionary<string, Func<ITemplateArgs<TSubProperty>, HelperResult>>();
            for (int it = 0; it < definedSections.Length; it++)
                sectionsDic.Add(definedSections[it], passedSections[it]);

            return sectionsDic;
        }

        /// <summary>
        /// Adds a table-level named section, that can be used when rendering the table.
        /// </summary>
        /// <param name="sectionName"> Name of the section. </param>
        /// <param name="section"> Section helper returning a HelperResult given the contextual template arguments. </param>
        /// <returns> The current <see cref="TableTemplate&lt;TModel, TCollectionItem&gt;"/> for fluent configuration. </returns>
        public TableTemplate<TModel, TCollectionItem> AddSection(
            [NotNull] string sectionName,
            [NotNull] Func<ITemplateArgs<IList<TCollectionItem>>, HelperResult> section)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (section == null)
                throw new ArgumentNullException("section");

            var sectionObj = new HelperResultTemplateSection<ITemplateArgs<IList<TCollectionItem>>>(section, null);
            this.rootSections.Add(sectionName, sectionObj);
            return this;
        }

        /// <summary>
        /// Adds an item-level named section, that can be used when rendering the table items.
        /// </summary>
        /// <param name="sectionName"> Name of the item-level section. </param>
        /// <param name="section"> Section helper returning a HelperResult given the contextual template arguments. </param>
        /// <returns> The current <see cref="TableTemplate&lt;TModel, TCollectionItem&gt;"/> for fluent configuration. </returns>
        public TableTemplate<TModel, TCollectionItem> AddItemSection(
            [NotNull] string sectionName,
            [NotNull] Func<ITemplateArgsWithData<TCollectionItem, RowInfo>, HelperResult> section)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (section == null)
                throw new ArgumentNullException("section");

            var sectionObj = new HelperResultTemplateSection<ITemplateArgsWithData<TCollectionItem, RowInfo>>(section, null);
            this.itemSections.Add(sectionName, sectionObj);
            return this;
        }

        /// <summary>
        /// Adds an item-level named section, that can be used when rendering the table items,
        /// which availability is conditional.
        /// </summary>
        /// <param name="sectionName"> Name of the item-level section.  </param>
        /// <param name="section"> Section helper returning a HelperResult given the contextual template arguments.  </param>
        /// <param name="predicate"> Predicate for the section availability. </param>
        /// <returns> The current <see cref="TableTemplate&lt;TModel, TCollectionItem&gt;"/> for fluent configuration.  </returns>
        public TableTemplate<TModel, TCollectionItem> AddItemSection(
            [NotNull] string sectionName,
            [NotNull] Func<ITemplateArgsWithData<TCollectionItem, RowInfo>, HelperResult> section,
            [NotNull] Func<ITemplateArgsWithData<TCollectionItem, RowInfo>, bool> predicate)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (section == null)
                throw new ArgumentNullException("section");

            var sectionObj = new HelperResultTemplateSection<ITemplateArgsWithData<TCollectionItem, RowInfo>>(section, predicate);
            this.itemSections.Add(sectionName, sectionObj);
            return this;
        }
    }
}