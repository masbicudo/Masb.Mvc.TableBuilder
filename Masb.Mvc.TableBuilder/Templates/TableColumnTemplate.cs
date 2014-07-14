using System;
using System.Linq.Expressions;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public class TableColumnTemplate<TCollectionItem, TSubProperty> :
        ITableColumnTemplate<TCollectionItem, TSubProperty>
    {
        private readonly Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression;
        private readonly Func<IViewTemplate, HelperResult> header;
        private readonly Func<IViewTemplate<TSubProperty>, HelperResult> content;

        public TableColumnTemplate(
            Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            Func<IViewTemplate, HelperResult> header,
            Func<IViewTemplate<TSubProperty>, HelperResult> content)
        {
            this.subPropertyExpression = subPropertyExpression;
            this.header = header;
            this.content = content;
        }

        public Expression<Func<TCollectionItem, TSubProperty>> Expression
        {
            get { return this.subPropertyExpression; }
        }

        public TResult Accept<TResult>(ITableColumnTemplateFromVisitor<TCollectionItem, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public HelperResult GetHeaderHelperResult(IViewTemplate<TSubProperty> viewTemplate)
        {
            var result = this.header(viewTemplate);
            return result;
        }

        public HelperResult GetDataHelperResult(IViewTemplate<TSubProperty> viewTemplate)
        {
            var result = this.content(viewTemplate);
            return result;
        }

        public TResult Accept<TResult>(ITableColumnTemplateToVisitor<TSubProperty, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public TResult Accept<TResult>(ITableColumnTemplateVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}