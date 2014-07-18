using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.WebPages;
using JetBrains.Annotations;

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

                if (this.masterHtml.ViewData.Model == null)
                    return Enumerable.Empty<ITableDataRowRenderer>();

                var collection = getter(this.masterHtml.ViewData.Model);

                if (collection == null)
                    return Enumerable.Empty<ITableDataRowRenderer>();

                var result = collection.Select((x, i) => this.CreateItem(x, i, false));
                return result;
            }
        }

        public ITableDataRowRenderer NewItem(int index, object defaultModel)
        {
            return this.CreateItem((TCollectionItem)defaultModel, index, true);
        }

        public ITableDataRowRenderer NewItem(int index)
        {
            return this.CreateItem(default(TCollectionItem), index, true);
        }

        [NotNull]
        public HelperResult RenderSection([NotNull] string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            return this.RenderSection(sectionName, true);
        }

        [ContractAnnotation("null <= required: false; notnull <= required: true")]
        public HelperResult RenderSection([NotNull] string sectionName, bool required)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            var helperResult = this.GetHelperResult(sectionName);

            if (helperResult == null && required)
                throw new Exception(string.Format("Section must be defined: {0}", sectionName));

            return helperResult;
        }

        public bool IsSectionDefined([NotNull] string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            return this.tableTemplate.IsSectionDefined(sectionName);
        }

        [CanBeNull]
        private HelperResult GetHelperResult([NotNull] string sectionName)
        {
            if (!this.tableTemplate.IsSectionDefined(sectionName))
                return null;

            var result = this.tableTemplate.GetSectionHelperResult(sectionName, this.lazyViewTemplate.Value);

            return result;
        }

        public TableDataRowRenderer<TCollectionItem> NewItem(int index, TCollectionItem item)
        {
            return this.CreateItem(item, index, true);
        }

        private TableDataRowRenderer<TCollectionItem> CreateItem(TCollectionItem item, int index, bool isNewRow)
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

                if (body.Type.IsArray)
                {
                    var indexer = Expression.ArrayIndex(
                        body,
                        Expression.Constant(index));

                    indexerLambda = Expression.Lambda<Func<TModel, TCollectionItem>>(indexer, lambda.Parameters);
                }
                else
                {
                    var indexerTypes = new[]
                    {
                        typeof(int),
                        typeof(uint),
                        typeof(long),
                        typeof(ulong),
                        typeof(ushort),
                        typeof(short),
                        typeof(sbyte),
                        typeof(byte)
                    };

                    // getting indexer get method:
                    // reference: http://stackoverflow.com/questions/6759416/accessing-indexer-from-expression-tree
                    var indexerPropInfos = (from pi in body.Type.GetDefaultMembers().OfType<PropertyInfo>()
                                            where pi.PropertyType == typeof(TCollectionItem) && pi.CanRead
                                            let args = pi.GetIndexParameters()
                                            where args.Length == 1 && args[0].ParameterType == typeof(int)
                                            let argsType = args[0].ParameterType
                                            where indexerTypes.Contains(argsType)
                                            select new { pi, argsType }).ToDictionary(x => x.argsType, x => x.pi);

                    var indexerPropInfo = indexerTypes.Select(
                        x =>
                        {
                            PropertyInfo pi;
                            indexerPropInfos.TryGetValue(x, out pi);
                            return pi;
                        })
                        .FirstOrDefault(pi => pi != null);

                    if (indexerPropInfo == null)
                        throw new Exception(string.Format(
                            "Indexer accepting an integer argument and returning {0} was not found in type {1}.",
                            typeof(TCollectionItem).Name,
                            body.Type.Name));

                    var indexer = Expression.Call(
                        body,
                        indexerPropInfo.GetGetMethod(),
                        new Expression[] { Expression.Constant(index) });

                    indexerLambda = Expression.Lambda<Func<TModel, TCollectionItem>>(indexer, lambda.Parameters);
                }
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

            var viewTemplate = new ViewTemplate<TCollectionItem, RowInfo>(viewData, viewContext, info: new RowInfo(index, isNewRow));

            var row = new TableDataRowRenderer<TCollectionItem>(
                this.tableTemplate,
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
            private readonly HtmlHelper masterHtml;

            public TableHeaderRowRendererCreator(HtmlHelper masterHtml)
            {
                this.masterHtml = masterHtml;
            }

            public TableHeaderRowRenderer Visit<TCollectionItem1>(ITableTemplate<TModel, TCollectionItem1> value)
            {
                var creator = new TableHeaderCellRendererCreator<TCollectionItem1>(this.masterHtml);
                var allColumns = value.Columns.Select(x => x.Accept(creator));
                var result = new TableHeaderRowRenderer(allColumns);
                return result;
            }

            private class TableHeaderCellRendererCreator<TCollectionItem1> :
                ITableColumnTemplateFromVisitor<TCollectionItem1, ITableHeaderCellRenderer>
            {
                private readonly HtmlHelper rootHtml;

                public TableHeaderCellRendererCreator(HtmlHelper rootHtml)
                {
                    this.rootHtml = rootHtml;
                }

                public ITableHeaderCellRenderer Visit<TSubProperty>(ITableColumnTemplate<TCollectionItem1, TSubProperty> value)
                {
                    var viewData = new ViewDataDictionary<TSubProperty>
                    {
                        //TemplateInfo =
                        //{
                        //    HtmlFieldPrefix = this.html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                        //        ExpressionHelper.GetExpressionText(value.Expression))
                        //},
                        ModelMetadata = ModelMetadata.FromLambdaExpression(
                            value.Expression,
                            new ViewDataDictionary<TCollectionItem1>())
                    };

                    var viewContext = new ViewContext(
                        this.rootHtml.ViewContext,
                        this.rootHtml.ViewContext.View,
                        viewData,
                        this.rootHtml.ViewContext.TempData,
                        this.rootHtml.ViewContext.Writer);

                    var viewTemplate = new ViewTemplate<TSubProperty>(viewData, viewContext);

                    var result = new TableHeaderCellRenderer<TSubProperty>(value, viewTemplate);
                    return result;
                }
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