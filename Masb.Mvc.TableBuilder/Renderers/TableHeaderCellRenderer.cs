using System.Web.Mvc;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public class TableHeaderCellRenderer
    {
        private readonly ITableColumnTemplate tableColumnTemplate;
        private readonly HtmlHelper html;

        public TableHeaderCellRenderer(ITableColumnTemplate tableColumnTemplate, HtmlHelper html)
        {
            this.tableColumnTemplate = tableColumnTemplate;
            this.html = html;
        }

        public HelperResult Render()
        {
            var result = this.tableColumnTemplate.Accept(new HelperResultCreator(this.html));
            return result;
        }

        private class HelperResultCreator :
            ITableColumnTemplateVisitor<HelperResult>
        {
            private readonly HtmlHelper html;

            public HelperResultCreator(HtmlHelper html)
            {
                this.html = html;
            }

            public HelperResult Visit<TCollectionItem, TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value)
            {
                var viewData = new ViewDataDictionary<TSubProperty>
                {
                    TemplateInfo =
                    {
                        HtmlFieldPrefix = this.html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                            ExpressionHelper.GetExpressionText(value.Expression))
                    },
                    ModelMetadata = ModelMetadata.FromLambdaExpression(
                        value.Expression,
                        new ViewDataDictionary<TCollectionItem>())
                };

                var viewContext = new ViewContext(
                    this.html.ViewContext,
                    this.html.ViewContext.View,
                    viewData,
                    this.html.ViewContext.TempData,
                    this.html.ViewContext.Writer);

                var viewTemplate = new ViewTemplate<TSubProperty>(viewData, viewContext);

                var result = value.GetHeaderHelperResult(viewTemplate);
                return result;
            }
        }
    }
}