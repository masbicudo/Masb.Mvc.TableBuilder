using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder
{
    public class TableRenderer<TModel, TCollectionItem> :
        ITableRenderer
    {
        private readonly ITableTemplate<TModel, TCollectionItem> tableTemplate;
        private readonly HtmlHelper<TModel> html;

        public TableRenderer(ITableTemplate<TModel, TCollectionItem> tableTemplate, HtmlHelper<TModel> html)
        {
            this.tableTemplate = tableTemplate;
            this.html = html;
        }

        public TableHeaderRowRenderer Header
        {
            get { return this.tableTemplate.Accept(new TableHeaderRowRendererCreator(this.html)); }
        }

        public IEnumerable<ITableDataRowRenderer> Items
        {
            get
            {
                var getter = this.tableTemplate.Expression.Compile();
                IEnumerable<TCollectionItem> collection = getter(this.html.ViewData.Model);

                var result = collection.Select(
                    (item, i) =>
                    {
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
                                new Expression[] { Expression.Constant(i) });

                            indexerLambda = Expression.Lambda<Func<TModel, TCollectionItem>>(indexer, lambda.Parameters);
                        }

                        var viewData = new ViewDataDictionary<TCollectionItem>(item)
                        {
                            TemplateInfo =
                            {
                                HtmlFieldPrefix = this.html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                                    ExpressionHelper.GetExpressionText(indexerLambda))
                            },
                            ModelMetadata = ModelMetadata.FromLambdaExpression(
                                indexerLambda,
                                this.html.ViewData)
                        };

                        var viewContext = new ViewContext(
                            this.html.ViewContext,
                            this.html.ViewContext.View,
                            viewData,
                            this.html.ViewContext.TempData,
                            this.html.ViewContext.Writer);

                        var viewTemplate = new ViewTemplate<TCollectionItem>(viewData, viewContext);

                        var row = new TableDataRowRenderer<TCollectionItem>(this.tableTemplate.Columns, viewTemplate.Html);
                        return row;
                    });
                return result;
            }
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
    }
}