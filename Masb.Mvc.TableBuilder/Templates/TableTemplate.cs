using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public class TableTemplate<TModel, TCollectionItem> :
        ITableTemplate<TModel, TCollectionItem>
    {
        private readonly Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression;

        private readonly List<ITableColumnTemplateFrom<TCollectionItem>> columns
            = new List<ITableColumnTemplateFrom<TCollectionItem>>();

        public TableTemplate(Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression)
        {
            this.collectionExpression = collectionExpression;
        }

        public TableTemplate<TModel, TCollectionItem> AddColumnFor<TSubProperty>(
            Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            Func<IViewTemplate, HelperResult> header,
            Func<IViewTemplate<TSubProperty>, HelperResult> content)
        {
            this.columns.Add(new TableColumnTemplate<TCollectionItem, TSubProperty>(subPropertyExpression, header, content));
            return this;
        }

        public TableTemplate<TModel, TCollectionItem> AddColumn(
            Func<IViewTemplate, HelperResult> header,
            Func<IViewTemplate<TCollectionItem>, HelperResult> content)
        {
            this.columns.Add(new TableColumnTemplate<TCollectionItem, TCollectionItem>(x => x, header, content));
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
    }
}