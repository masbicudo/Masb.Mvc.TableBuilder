using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public class TableTemplate<TModel, TCollectionItem> :
        ITableTemplate<TModel, TCollectionItem>
    {
        private readonly Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression;

        private readonly List<ITableColumnTemplateFrom<TCollectionItem>> columns
            = new List<ITableColumnTemplateFrom<TCollectionItem>>();

        private readonly Dictionary<string, Func<IViewTemplate<IList<TCollectionItem>>, HelperResult>> rootSections
            = new Dictionary<string, Func<IViewTemplate<IList<TCollectionItem>>, HelperResult>>();

        private readonly Dictionary<string, Func<IViewTemplateWithData<TCollectionItem, RowInfo>, HelperResult>> itemSections
            = new Dictionary<string, Func<IViewTemplateWithData<TCollectionItem, RowInfo>, HelperResult>>();

        private string[] columnSectioNames;

        public TableTemplate(Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression)
        {
            this.collectionExpression = collectionExpression;
        }

        public TableTemplate<TModel, TCollectionItem> AddColumnFor<TSubProperty>(
            Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            Func<IViewTemplate, HelperResult> header,
            Func<IViewTemplate<TSubProperty>, HelperResult> content,
            params Func<IViewTemplate<TSubProperty>, HelperResult>[] sections)
        {
            var sectionsDic = GetSectionsDictionary(sections);
            this.columns.Add(new TableColumnTemplate<TCollectionItem, TSubProperty>(subPropertyExpression, header, content, sectionsDic));
            return this;
        }

        public TableTemplate<TModel, TCollectionItem> AddColumn(
            Func<IViewTemplate, HelperResult> header,
            Func<IViewTemplate<TCollectionItem>, HelperResult> content,
            params Func<IViewTemplate<TCollectionItem>, HelperResult>[] sections)
        {
            var sectionsDic = GetSectionsDictionary(sections);
            this.columns.Add(new TableColumnTemplate<TCollectionItem, TCollectionItem>(x => x, header, content, sectionsDic));
            return this;
        }

        public TResult Accept<TResult>(ITableTemplateVisitor<TModel, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public TResult Accept<TResult>(ITableTemplateToVisitor<TCollectionItem, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> Columns
        {
            get { return new ReadOnlyCollection<ITableColumnTemplateFrom<TCollectionItem>>(this.columns); }
        }

        public bool IsSectionDefined([NotNull] string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            Func<IViewTemplate<IList<TCollectionItem>>, HelperResult> xpto;
            var result = this.rootSections.TryGetValue(sectionName, out xpto) && xpto != null;
            return result;
        }

        [CanBeNull]
        public HelperResult GetSectionHelperResult(
            [NotNull] string sectionName,
            IViewTemplate<IList<TCollectionItem>> viewTemplate)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            Func<IViewTemplate<IList<TCollectionItem>>, HelperResult> xpto;
            if (!this.rootSections.TryGetValue(sectionName, out xpto) || xpto == null)
                return null;

            var result = xpto(viewTemplate);
            return result;
        }

        public bool IsItemSectionDefined([NotNull] string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            Func<IViewTemplateWithData<TCollectionItem, RowInfo>, HelperResult> xpto;
            var result = this.itemSections.TryGetValue(sectionName, out xpto) && xpto != null;
            return result;
        }

        [CanBeNull]
        public HelperResult GetItemSectionHelperResult(
            [NotNull] string sectionName,
            IViewTemplateWithData<TCollectionItem, RowInfo> viewTemplate)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            Func<IViewTemplateWithData<TCollectionItem, RowInfo>, HelperResult> xpto;
            if (!this.itemSections.TryGetValue(sectionName, out xpto) || xpto == null)
                return null;

            var result = xpto(viewTemplate);
            return result;
        }

        IEnumerable<ITableColumnTemplate> ITableTemplate.Columns
        {
            get { return this.Columns; }
        }

        public TResult Accept<TResult>(ITableTemplateVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public Expression<Func<TModel, IList<TCollectionItem>>> Expression
        {
            get { return this.collectionExpression; }
        }

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

        private Dictionary<string, Func<IViewTemplate<TSubProperty>, HelperResult>> GetSectionsDictionary<TSubProperty>(Func<IViewTemplate<TSubProperty>, HelperResult>[] sections)
        {
            var definedSections = this.columnSectioNames ?? new string[0];
            var passedSections = sections ?? new Func<IViewTemplate<TSubProperty>, HelperResult>[0];
            if (definedSections.Length != passedSections.Length)
                throw new Exception("Number of defined sections is different from the number of passed sections.");

            var sectionsDic = new Dictionary<string, Func<IViewTemplate<TSubProperty>, HelperResult>>();
            for (int it = 0; it < definedSections.Length; it++)
                sectionsDic.Add(definedSections[it], passedSections[it]);

            return sectionsDic;
        }

        public TableTemplate<TModel, TCollectionItem> AddSection(
            [NotNull] string sectionName,
            [NotNull] Func<IViewTemplate<IList<TCollectionItem>>, HelperResult> section)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (section == null)
                throw new ArgumentNullException("section");

            this.rootSections.Add(sectionName, section);
            return this;
        }

        public TableTemplate<TModel, TCollectionItem> AddItemSection(
            [NotNull] string sectionName,
            [NotNull] Func<IViewTemplateWithData<TCollectionItem, RowInfo>, HelperResult> section)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (section == null)
                throw new ArgumentNullException("section");

            this.itemSections.Add(sectionName, section);
            return this;
        }

        public TableTemplate<TModel, TCollectionItem> AddItemSection(
            [NotNull] string sectionName,
            [NotNull] Func<IViewTemplateWithData<TCollectionItem, RowInfo>, HelperResult> section,
            [NotNull] Func<IViewTemplateWithData<TCollectionItem, RowInfo>, bool> predicate)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (section == null)
                throw new ArgumentNullException("section");

            this.itemSections.Add(sectionName, vt => predicate(vt) ? section(vt) : null);
            return this;
        }
    }
}