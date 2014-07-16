using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public class TableColumnTemplate<TCollectionItem, TSubProperty> :
        ITableColumnTemplate<TCollectionItem, TSubProperty>
    {
        private readonly Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression;
        private readonly Func<IViewTemplate, HelperResult> header;
        private readonly Func<IViewTemplate<TSubProperty>, HelperResult> content;
        private readonly Dictionary<string, Func<IViewTemplate<TSubProperty>, HelperResult>> sections;

        public TableColumnTemplate(
            Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            Func<IViewTemplate, HelperResult> header,
            Func<IViewTemplate<TSubProperty>, HelperResult> content,
            Dictionary<string, Func<IViewTemplate<TSubProperty>, HelperResult>> sections)
        {
            this.subPropertyExpression = subPropertyExpression;
            this.header = header;
            this.content = content;
            this.sections = sections;
        }

        public Expression<Func<TCollectionItem, TSubProperty>> Expression
        {
            get { return this.subPropertyExpression; }
        }

        public TResult Accept<TResult>(ITableColumnTemplateFromVisitor<TCollectionItem, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        [CanBeNull]
        public HelperResult GetSectionResult([NotNull] string sectionName, IViewTemplate<TSubProperty> viewTemplate)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            Func<IViewTemplate<TSubProperty>, HelperResult> sectionFunc;
            if (!this.sections.TryGetValue(sectionName, out sectionFunc))
                return null;

            var result = sectionFunc(viewTemplate);
            return result;
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

        public bool IsSectionDefined(string sectionName)
        {
            Func<IViewTemplate<TSubProperty>, HelperResult> xpto;
            return this.sections.TryGetValue(sectionName, out xpto) && xpto != null;
        }

        public HelperResult GetSectionHelperResult(string sectionName, IViewTemplate<TSubProperty> viewTemplate)
        {
            Func<IViewTemplate<TSubProperty>, HelperResult> xpto;
            if (!this.sections.TryGetValue(sectionName, out xpto) || xpto == null)
                return null;

            var result = xpto(viewTemplate);
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