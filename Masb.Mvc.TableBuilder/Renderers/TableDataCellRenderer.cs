using System.Web.Mvc;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder.Code
{
    public class TableDataCellRenderer<TCollectionItem>
    {
        private readonly ITableColumnTemplateFrom<TCollectionItem> tableColumnTemplate;
        private readonly HtmlHelper<TCollectionItem> html;

        public TableDataCellRenderer(ITableColumnTemplateFrom<TCollectionItem> tableColumnTemplate, HtmlHelper<TCollectionItem> html)
        {
            this.tableColumnTemplate = tableColumnTemplate;
            this.html = html;
        }

        public HelperResult Render()
        {
            var result = this.tableColumnTemplate.Accept(new HelperResultCreator(this.html));
            return result;
        }

        class HelperResultCreator : ITableColumnTemplateFromVisitor<TCollectionItem, HelperResult>
        {
            private readonly HtmlHelper<TCollectionItem> html;

            public HelperResultCreator(HtmlHelper<TCollectionItem> html)
            {
                this.html = html;
            }

            public HelperResult Visit<TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value)
            {
                var model = value.Expression.Compile()(this.html.ViewData.Model);

                var viewData = new ViewDataDictionary<TSubProperty>(model)
                {
                    TemplateInfo =
                    {
                        HtmlFieldPrefix = this.html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                            ExpressionHelper.GetExpressionText(value.Expression))
                    },
                    ModelMetadata = ModelMetadata.FromLambdaExpression(
                        value.Expression,
                        this.html.ViewData)
                };

                var viewContext = new ViewContext(
                    this.html.ViewContext,
                    this.html.ViewContext.View,
                    viewData,
                    this.html.ViewContext.TempData,
                    this.html.ViewContext.Writer);

                var viewTemplate = new ViewTemplate<TSubProperty>(viewData, viewContext);

                var result = value.GetDataHelperResult(viewTemplate);
                return result;
            }
        }
    }
}