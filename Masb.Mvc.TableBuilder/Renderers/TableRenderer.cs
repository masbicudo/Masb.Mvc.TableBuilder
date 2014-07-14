using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder
{
    public class TableRenderer<TModel, TCollectionItem> :
        IViewTemplate<IList<TCollectionItem>>,
        ITableRenderer
    {
        private readonly ITableTemplate<TModel, TCollectionItem> tableTemplate;
        private readonly HtmlHelper<TModel> masterHtml;

        public TableRenderer(ITableTemplate<TModel, TCollectionItem> tableTemplate, HtmlHelper<TModel> masterHtml)
        {
            this.tableTemplate = tableTemplate;
            this.masterHtml = masterHtml;
            this.lazyViewTemplate = new Lazy<ViewTemplate<IList<TCollectionItem>>>(this.CreateInnerViewTemplate);
        }

        public TableHeaderRowRenderer Header
        {
            get { return this.tableTemplate.Accept(new TableHeaderRowRendererCreator(this.masterHtml)); }
        }

        public IEnumerable<ITableDataRowRenderer> Items
        {
            get
            {
                var getter = this.tableTemplate.Expression.Compile();
                var collection = getter(this.masterHtml.ViewData.Model);
                var result = collection.Select(this.CreateItem);
                return result;
            }
        }

        public ITableDataRowRenderer NewItem(int index, object defaultModel)
        {
            return this.CreateItem((TCollectionItem)defaultModel, index);
        }

        public ITableDataRowRenderer NewItem(int index)
        {
            return this.CreateItem(default(TCollectionItem), index);
        }

        public TableDataRowRenderer<TCollectionItem> NewItem(int index, TCollectionItem item)
        {
            return this.CreateItem(item, index);
        }

        private TableDataRowRenderer<TCollectionItem> CreateItem(TCollectionItem item, int index)
        {
            var indexHiddenFieldName = this.masterHtml.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                ExpressionHelper.GetExpressionText(this.tableTemplate.Expression)) + ".Index";

            var indexHiddenElementId = this.masterHtml.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(
                ExpressionHelper.GetExpressionText(this.tableTemplate.Expression)) + "_Index__" + index;

            Expression<Func<TModel, TCollectionItem>> indexerLambda;
            {
                var expression = this.tableTemplate.Expression;
                var lambda = expression as LambdaExpression;
                var body = lambda.Body;
                if (body.NodeType == ExpressionType.Convert)
                    body = ((UnaryExpression)body).Operand;

                var indexer = Expression.Call(
                    body,
                    // TODO: the indexer is not always named "Item"
                    "get_Item",
                    new Type[0],
                    new Expression[] { Expression.Constant(index) });

                indexerLambda = Expression.Lambda<Func<TModel, TCollectionItem>>(indexer, lambda.Parameters);
            }

            var viewData = new ViewDataDictionary<TCollectionItem>(item)
            {
                TemplateInfo =
                {
                    HtmlFieldPrefix = this.masterHtml.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                        ExpressionHelper.GetExpressionText(indexerLambda))
                },
                ModelMetadata = ModelMetadata.FromLambdaExpression(
                    indexerLambda,
                    this.masterHtml.ViewData)
            };

            var viewContext = new ViewContext(
                this.masterHtml.ViewContext,
                this.masterHtml.ViewContext.View,
                viewData,
                this.masterHtml.ViewContext.TempData,
                this.masterHtml.ViewContext.Writer);

            var viewTemplate = new ViewTemplate<TCollectionItem>(viewData, viewContext);

            var row = new TableDataRowRenderer<TCollectionItem>(
                this.tableTemplate.Columns,
                viewTemplate,
                index,
                indexHiddenFieldName,
                indexHiddenElementId);

            return row;
        }

        private class TableHeaderRowRendererCreator :
            ITableTemplateVisitor<TModel, TableHeaderRowRenderer>
        {
            private readonly HtmlHelper html;

            public TableHeaderRowRendererCreator(HtmlHelper html)
            {
                this.html = html;
            }

            public TableHeaderRowRenderer Visit<TCollectionItem1>(ITableTemplate<TModel, TCollectionItem1> value)
            {
                var allColumns = value.Columns.Select(x => new TableHeaderCellRenderer(x, this.html));
                var result = new TableHeaderRowRenderer(allColumns);
                return result;
            }
        }

        private readonly Lazy<ViewTemplate<IList<TCollectionItem>>> lazyViewTemplate;

        public ViewTemplate<IList<TCollectionItem>> CreateInnerViewTemplate()
        {
            var getter = this.tableTemplate.Expression.Compile();
            var collection = getter(this.masterHtml.ViewData.Model);
            var viewData = new ViewDataDictionary<IList<TCollectionItem>>(collection)
            {
                TemplateInfo =
                {
                    HtmlFieldPrefix = this.masterHtml.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                        ExpressionHelper.GetExpressionText(this.tableTemplate.Expression))
                },
                ModelMetadata = ModelMetadata.FromLambdaExpression(
                    this.tableTemplate.Expression,
                    this.masterHtml.ViewData)
            };

            var viewContext = new ViewContext(
                this.masterHtml.ViewContext,
                this.masterHtml.ViewContext.View,
                viewData,
                this.masterHtml.ViewContext.TempData,
                this.masterHtml.ViewContext.Writer);

            var viewTemplate = new ViewTemplate<IList<TCollectionItem>>(viewData, viewContext);

            return viewTemplate;
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        public AjaxHelper<IList<TCollectionItem>> Ajax
        {
            get { return this.lazyViewTemplate.Value.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper<IList<TCollectionItem>> IViewTemplate<IList<TCollectionItem>>.Html
        {
            get { return this.lazyViewTemplate.Value.Html; }
        }

        /// <summary>
        /// Gets the current model object or value.
        /// </summary>
        public IList<TCollectionItem> Model
        {
            get { return this.lazyViewTemplate.Value.Model; }
        }

        public ModelMetadata Meta
        {
            get { return this.lazyViewTemplate.Value.Meta; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.UrlHelper"/> of the rendered snippet.
        /// </summary>
        public UrlHelper Url
        {
            get { return this.lazyViewTemplate.Value.Url; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        AjaxHelper IViewTemplate.Ajax
        {
            get { return this.lazyViewTemplate.Value.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper IViewTemplate.Html
        {
            get { return this.lazyViewTemplate.Value.Html; }
        }
    }
}