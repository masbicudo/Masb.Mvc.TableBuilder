using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public static class HtmlHelperExtensions
    {
        [NotNull]
        public static TableTemplate<TModel, TCollectionItem> TableTemplateFor<TModel, TCollectionItem>(
            [NotNull] this HtmlHelper<TModel> html,
            Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression)
        {
            if (html == null)
                throw new ArgumentNullException("html");

            return new TableTemplate<TModel, TCollectionItem>(collectionExpression);
        }

        [NotNull]
        public static TableRenderer<TModel, TCollectionItem> Table<TModel, TCollectionItem>(
            this HtmlHelper<TModel> html,
            ITableTemplate<TModel, TCollectionItem> tableTemplate)
        {
            return new TableRenderer<TModel, TCollectionItem>(tableTemplate, html);
        }

        [NotNull]
        public static ITableRenderer Table<TModel>(
            this HtmlHelper<TModel> html,
            ITableTemplate<TModel> tableTemplate)
        {
            return tableTemplate.Accept(new TableRendererCreator<TModel>(html));
        }

        [NotNull]
        public static ITableRenderer Table(
            this HtmlHelper html,
            ITableTemplate tableTemplate)
        {
            return tableTemplate.Accept(new TableRendererCreator(html));
        }

        private class TableRendererCreator :
            ITableTemplateVisitor<ITableRenderer>
        {
            private readonly HtmlHelper html;

            public TableRendererCreator([NotNull] HtmlHelper html)
            {
                this.html = html;
            }

            [NotNull]
            public ITableRenderer Visit<TModel, TCollectionItem>(ITableTemplate<TModel, TCollectionItem> value)
            {
                return new TableRenderer<TModel, TCollectionItem>(value, (HtmlHelper<TModel>)this.html);
            }
        }

        private class TableRendererCreator<TModel> :
            ITableTemplateVisitor<TModel, ITableRenderer>
        {
            private readonly HtmlHelper<TModel> html;

            public TableRendererCreator([NotNull] HtmlHelper<TModel> html)
            {
                this.html = html;
            }

            [NotNull]
            public ITableRenderer Visit<TCollectionItem>(ITableTemplate<TModel, TCollectionItem> value)
            {
                return new TableRenderer<TModel, TCollectionItem>(value, this.html);
            }
        }
    }
}