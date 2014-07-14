using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder.Code
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

    public class TableTemplate<TModel, TCollectionItem> : ITableTemplate<TModel, TCollectionItem>
    {
        private readonly Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression;

        private readonly List<ITableColumnTemplateFrom<TCollectionItem>> columns
            = new List<ITableColumnTemplateFrom<TCollectionItem>>();

        public TableTemplate(Expression<Func<TModel, IList<TCollectionItem>>> collectionExpression)
        {
            this.collectionExpression = collectionExpression;
        }

        public TableTemplate<TModel, TCollectionItem> AddColumn<TSubProperty>(
            Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            Func<IViewTemplate, HelperResult> header,
            Func<IViewTemplate<TSubProperty>, HelperResult> content)
        {
            this.columns.Add(new TableColumnTemplate<TCollectionItem, TSubProperty>(subPropertyExpression, header, content));
            return this;
        }

        public TResult Accept<TResult>(ITableTemplateVisitor<TModel, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public TResult Accept<TResult>(ITableTemplateToVisitor<TCollectionItem, TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> Columns
        {
            get { return new ReadOnlyCollection<ITableColumnTemplateFrom<TCollectionItem>>(this.columns); }
        }

        IEnumerable<ITableColumnTemplate> ITableTemplate.Columns
        {
            get { return this.Columns; }
        }

        public TResult Accept<TResult>(ITableTemplateVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public Expression<Func<TModel, IList<TCollectionItem>>> Expression
        {
            get { return this.collectionExpression; }
        }
    }

    public interface ITableTemplate<TModel, TCollectionItem> :
        ITableTemplate<TModel>,
        ITableTemplateTo<TCollectionItem>
    {
        Expression<Func<TModel, IList<TCollectionItem>>> Expression { get; }
    }

    public interface ITableTemplate<TModel> :
        ITableTemplate
    {
        TResult Accept<TResult>(ITableTemplateVisitor<TModel, TResult> visitor);
    }

    public interface ITableTemplateVisitor<TModel, out TResult>
    {
        TResult Visit<TCollectionItem>(ITableTemplate<TModel, TCollectionItem> value);
    }

    public interface ITableTemplateTo<TCollectionItem> :
        ITableTemplate
    {
        TResult Accept<TResult>(ITableTemplateToVisitor<TCollectionItem, TResult> visitor);

        new IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> Columns { get; }
    }

    public interface ITableTemplateToVisitor<TCollectionItem, out TResult>
    {
        TResult Visit<TModel>(ITableTemplate<TModel, TCollectionItem> value);
    }

    public interface ITableTemplate
    {
        IEnumerable<ITableColumnTemplate> Columns { get; }

        TResult Accept<TResult>(ITableTemplateVisitor<TResult> visitor);
    }

    public interface ITableTemplateVisitor<out TResult>
    {
        TResult Visit<TModel, TCollectionItem>(ITableTemplate<TModel, TCollectionItem> value);
    }

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

    public interface ITableColumnTemplate<TCollectionItem, TSubProperty> :
        ITableColumnTemplateFrom<TCollectionItem>,
        ITableColumnTemplateTo<TSubProperty>
    {
        Expression<Func<TCollectionItem, TSubProperty>> Expression { get; }
    }

    public interface ITableColumnTemplateFrom<TCollectionItem> :
        ITableColumnTemplate
    {
        TResult Accept<TResult>(ITableColumnTemplateFromVisitor<TCollectionItem, TResult> visitor);
    }

    public interface ITableColumnTemplateFromVisitor<TCollectionItem, out TResult>
    {
        TResult Visit<TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value);
    }

    public interface ITableColumnTemplateTo<TSubProperty> :
        ITableColumnTemplate
    {
        HelperResult GetHeaderHelperResult(IViewTemplate<TSubProperty> viewTemplate);

        HelperResult GetDataHelperResult(IViewTemplate<TSubProperty> viewTemplate);

        TResult Accept<TResult>(ITableColumnTemplateToVisitor<TSubProperty, TResult> visitor);
    }

    public interface ITableColumnTemplateToVisitor<TSubProperty, out TResult>
    {
        TResult Visit<TCollectionItem>(ITableColumnTemplate<TCollectionItem, TSubProperty> value);
    }

    public interface ITableColumnTemplate
    {
        TResult Accept<TResult>(ITableColumnTemplateVisitor<TResult> visitor);
    }

    public interface ITableColumnTemplateVisitor<out TResult>
    {
        TResult Visit<TCollectionItem, TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value);
    }

    public class TableRenderer<TModel, TCollectionItem>
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

        public IEnumerable<TableDataRowRenderer<TCollectionItem>> Items
        {
            get
            {
                var getter = this.tableTemplate.Expression.Compile();
                IEnumerable<TCollectionItem> collection = getter(this.html.ViewData.Model);

                var result = collection.Select(
                    (item, i) =>
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
                            new[] { Expression.Constant(i) });
                        var indexerLambda = Expression.Lambda<Func<TModel, TCollectionItem>>(indexer, lambda.Parameters);

                        var viewData = new ViewDataDictionary<TCollectionItem>(item);
                        viewData.TemplateInfo.HtmlFieldPrefix =
                            this.html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                                ExpressionHelper.GetExpressionText((LambdaExpression)indexerLambda));
                        viewData.ModelMetadata = ModelMetadata.FromLambdaExpression(
                            indexerLambda,
                            this.html.ViewData);

                        var viewTemplate = new ViewTemplate<TCollectionItem>
                        {
                            ViewData = viewData,
                            Meta = viewData.ModelMetadata
                        };

                        var viewContext = new ViewContext(
                            this.html.ViewContext,
                            this.html.ViewContext.View,
                            viewData,
                            this.html.ViewContext.TempData,
                            this.html.ViewContext.Writer);

                        var html2 = new HtmlHelper<TCollectionItem>(viewContext, viewTemplate);

                        var row = new TableDataRowRenderer<TCollectionItem>(this.tableTemplate.Columns, html2);
                        return row;
                    });
                return result;
            }
        }

        class Replace : ExpressionVisitor
        {
            private readonly Expression replacement;

            public Replace(Expression replacement)
            {
                this.replacement = replacement;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return this.replacement;
            }
        }

        class TableHeaderRowRendererCreator : ITableTemplateVisitor<TModel, TableHeaderRowRenderer>
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

    public class TableHeaderRowRenderer
    {
        private readonly IEnumerable<TableHeaderCellRenderer> columns;

        public TableHeaderRowRenderer(IEnumerable<TableHeaderCellRenderer> columns)
        {
            this.columns = columns;
        }

        public IEnumerable<TableHeaderCellRenderer> Cells
        {
            get { return this.columns; }
        }
    }

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

        class HelperResultCreator : ITableColumnTemplateVisitor<HelperResult>
        {
            private readonly HtmlHelper html;

            public HelperResultCreator(HtmlHelper html)
            {
                this.html = html;
            }

            public HelperResult Visit<TCollectionItem, TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value)
            {
                var viewData = new ViewDataDictionary<TSubProperty>();
                viewData.TemplateInfo.HtmlFieldPrefix =
                    this.html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                        ExpressionHelper.GetExpressionText((LambdaExpression)value.Expression));
                viewData.ModelMetadata = ModelMetadata.FromLambdaExpression(
                    value.Expression,
                    new ViewDataDictionary<TCollectionItem>());

                var viewTemplate = new ViewTemplate<TSubProperty>();
                viewTemplate.ViewData = viewData;
                viewTemplate.Meta = viewData.ModelMetadata;

                var viewContext = new ViewContext(
                    this.html.ViewContext,
                    this.html.ViewContext.View,
                    viewData,
                    this.html.ViewContext.TempData,
                    this.html.ViewContext.Writer);

                viewTemplate.Html = new HtmlHelper<TSubProperty>(viewContext, viewTemplate);

                var result = value.GetHeaderHelperResult(viewTemplate);
                return result;
            }
        }
    }

    public class TableDataRowRenderer<TCollectionItem> :
        ITableDataRenderer<TCollectionItem>
    {
        private readonly IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> columns;
        private readonly HtmlHelper<TCollectionItem> html;

        public TableDataRowRenderer(IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> columns, HtmlHelper<TCollectionItem> html)
        {
            this.columns = columns;
            this.html = html;
        }

        public IEnumerable<TableDataCellRenderer<TCollectionItem>> Cells
        {
            get
            {
                var result = this.columns.Select(col => new TableDataCellRenderer<TCollectionItem>(col, this.html));
                return result;
            }
        }
    }

    public interface ITableDataRenderer<TCollectionItem> :
        ITableDataRenderer
    {
    }

    public interface ITableDataRenderer
    {
    }

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

                var viewData = new ViewDataDictionary<TSubProperty>(model);
                viewData.TemplateInfo.HtmlFieldPrefix =
                    this.html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                        ExpressionHelper.GetExpressionText((LambdaExpression)value.Expression));
                viewData.ModelMetadata = ModelMetadata.FromLambdaExpression(
                    value.Expression,
                    this.html.ViewData);

                var viewTemplate = new ViewTemplate<TSubProperty>
                                   {
                                       ViewData = viewData,
                                       Meta = viewData.ModelMetadata
                                   };

                var viewContext = new ViewContext(
                    this.html.ViewContext,
                    this.html.ViewContext.View,
                    viewData,
                    this.html.ViewContext.TempData,
                    this.html.ViewContext.Writer);

                viewTemplate.Html = new HtmlHelper<TSubProperty>(viewContext, viewTemplate);

                var result = value.GetDataHelperResult(viewTemplate);
                return result;
            }
        }
    }

    public class ViewTemplate<TModel> : IViewTemplate<TModel>, IViewDataContainer
    {
        public ModelMetadata Meta { get; set; }
        public HtmlHelper<TModel> Html { get; set; }

        HtmlHelper IViewTemplate.Html
        {
            get { return this.Html; }
            set { this.Html = (HtmlHelper<TModel>)value; }
        }

        public ViewDataDictionary ViewData { get; set; }
    }

    public interface IViewTemplate<TModel> : IViewTemplate
    {
        new HtmlHelper<TModel> Html { get; set; }
    }

    public interface IViewTemplate
    {
        ModelMetadata Meta { get; set; }

        HtmlHelper Html { get; set; }
    }
}