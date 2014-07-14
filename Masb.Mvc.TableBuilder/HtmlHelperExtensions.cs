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
    }
}