using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder
{
    public static class HtmlHelperExtensions
    {
        public static TableTemplate<TModel, TCollectionItem> TableTemplateFor<TModel, TCollectionItem>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression)
        {
            return new TableTemplate<TModel, TCollectionItem>(collectionExpression);
        }

        public static TableRenderer<TModel, TCollectionItem> Table<TModel, TCollectionItem>(
            this HtmlHelper<TModel> html,
            ITableTemplate<TModel, TCollectionItem> tableTemplate)
        {
            return new TableRenderer<TModel, TCollectionItem>(tableTemplate, html);
        }

        public static ITableRenderer Table<TModel>(
            this HtmlHelper<TModel> html,
            ITableTemplate<TModel> tableTemplate)
        {
            return tableTemplate.Accept(new TableRendererCreator<TModel>(html));
        }

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

            public TableRendererCreator(HtmlHelper html)
            {
                this.html = html;
            }

            public ITableRenderer Visit<TModel, TCollectionItem>(ITableTemplate<TModel, TCollectionItem> value)
            {
                return new TableRenderer<TModel, TCollectionItem>(value, (HtmlHelper<TModel>)html);
            }
        }

        private class TableRendererCreator<TModel> :
            ITableTemplateVisitor<TModel, ITableRenderer>
        {
            private readonly HtmlHelper<TModel> html;

            public TableRendererCreator(HtmlHelper<TModel> html)
            {
                this.html = html;
            }

            public ITableRenderer Visit<TCollectionItem>(ITableTemplate<TModel, TCollectionItem> value)
            {
                return new TableRenderer<TModel, TCollectionItem>(value, html);
            }
        }
    }
}