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
        private readonly Func<ITemplateArgs, HelperResult> header;
        private readonly Func<ITemplateArgs<TSubProperty>, HelperResult> content;
        private readonly Dictionary<string, Func<ITemplateArgs<TSubProperty>, HelperResult>> sections;

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

        public Expression<Func<TCollectionItem, TSubProperty>> Expression
        {
            get { return this.subPropertyExpression; }
        }

        public TResult Accept<TResult>(ITableColumnTemplateFromVisitor<TCollectionItem, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        [CanBeNull]
        public HelperResult GetSectionResult([NotNull] string sectionName, ITemplateArgs<TSubProperty> templateArgs)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            Func<ITemplateArgs<TSubProperty>, HelperResult> sectionFunc;
            if (!this.sections.TryGetValue(sectionName, out sectionFunc))
                return null;

            var result = sectionFunc(templateArgs);
            return result;
        }

        public HelperResult GetHeaderHelperResult(ITemplateArgs<TSubProperty> templateArgs)
        {
            var result = this.header(templateArgs);
            return result;
        }

        public HelperResult GetDataHelperResult(ITemplateArgs<TSubProperty> templateArgs)
        {
            var result = this.content(templateArgs);
            return result;
        }

        public bool IsSectionDefined(string sectionName)
        {
            Func<ITemplateArgs<TSubProperty>, HelperResult> xpto;
            return this.sections.TryGetValue(sectionName, out xpto) && xpto != null;
        }

        public HelperResult GetSectionHelperResult(string sectionName, ITemplateArgs<TSubProperty> templateArgs)
        {
            Func<ITemplateArgs<TSubProperty>, HelperResult> xpto;
            if (!this.sections.TryGetValue(sectionName, out xpto) || xpto == null)
                return null;

            var result = xpto(templateArgs);
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