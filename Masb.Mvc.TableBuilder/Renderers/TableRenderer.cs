using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a table rendering helper, that renders table templates.
    /// </summary>
    /// <typeparam name="TModel">Type of the root model.</typeparam>
    /// <typeparam name="TCollectionItem">Type of the collection item, rendered as data rows.</typeparam>
    public class TableRenderer<TModel, TCollectionItem> :
        IHelperContext<IList<TCollectionItem>>,
        ITableRenderer
    {
        [NotNull]
        private readonly ITableTemplate<TModel, TCollectionItem> tableTemplate;

        [NotNull]
        private readonly HtmlHelper<TModel> masterHtml;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableRenderer{TModel,TCollectionItem}"/> class.
        /// </summary>
        /// <param name="tableTemplate"> Table template to render. </param>
        /// <param name="masterHtml"> The master <see cref="HtmlHelper"/>. </param>
        public TableRenderer(
            [NotNull] ITableTemplate<TModel, TCollectionItem> tableTemplate,
            [NotNull] HtmlHelper<TModel> masterHtml)
        {
            if (tableTemplate == null)
                throw new ArgumentNullException("tableTemplate");

            if (masterHtml == null)
                throw new ArgumentNullException("masterHtml");

            this.tableTemplate = tableTemplate;
            this.masterHtml = masterHtml;
            this.lazyViewTemplate = new Lazy<TemplateArgs<IList<TCollectionItem>>>(this.CreateTemplateArgs);
        }

        /// <summary>
        /// Gets the header renderer.
        /// </summary>
        public TableHeaderRowRenderer Header
        {
            get { return this.tableTemplate.Accept(new TableHeaderRowRendererCreator(this.masterHtml)); }
        }

        /// <summary>
        /// Gets a renderer for each item in the table.
        /// </summary>
        public IEnumerable<ITableDataRowRenderer> Items
        {
            get
            {
                Debug.Assert(this.tableTemplate.Expression != null, "this.tableTemplate.Expression != null");

                var getter = this.tableTemplate.Expression.Compile();

                // ReSharper disable once CompareNonConstrainedGenericWithNull
                if (this.masterHtml.ViewData.Model == null)
                    return Enumerable.Empty<ITableDataRowRenderer>();

                var collection = getter(this.masterHtml.ViewData.Model);

                if (collection == null)
                    return Enumerable.Empty<ITableDataRowRenderer>();

                var result = collection.Select((x, i) => this.CreateItem(x, i, false));
                return result;
            }
        }

        /// <summary>
        /// Returns a renderer for a row that allows the insertion of new items.
        /// </summary>
        /// <param name="index">Index that is going to be considered as a new item.</param>
        /// <param name="defaultModel">Default model that will be used to render this new row.</param>
        /// <returns>A renderer that helps rendering the "new line".</returns>
        public ITableDataRowRenderer NewItem(int index, object defaultModel)
        {
            return this.CreateItem((TCollectionItem)defaultModel, index, true);
        }

        /// <summary>
        /// Returns a renderer for a row that allows the insertion of new items.
        /// </summary>
        /// <param name="index">Index that is going to be considered as a new item.</param>
        /// <returns>A renderer that helps rendering the "new line".</returns>
        public ITableDataRowRenderer NewItem(int index)
        {
            return this.CreateItem(default(TCollectionItem), index, true);
        }

        /// <summary>
        /// Returns a renderer for a row that allows the insertion of new items.
        /// </summary>
        /// <param name="index">Index that is going to be considered as a new item.</param>
        /// <param name="defaultModel">Default model that will be used to render this new row.</param>
        /// <returns>A renderer that helps rendering the "new line".</returns>
        [NotNull]
        public TableDataRowRenderer<TCollectionItem> NewItem(int index, TCollectionItem defaultModel)
        {
            return this.CreateItem(defaultModel, index, true);
        }

        [NotNull]
        private TableDataRowRenderer<TCollectionItem> CreateItem(TCollectionItem item, int index, bool isNewRow)
        {
            Debug.Assert(this.tableTemplate.Expression != null, "this.tableTemplate.Expression != null");

            var indexHiddenFieldName = this.masterHtml.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                ExpressionHelper.GetExpressionText(this.tableTemplate.Expression)) + ".Index";

            var indexHiddenElementId = this.masterHtml.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(
                ExpressionHelper.GetExpressionText(this.tableTemplate.Expression)) + "_Index__" + index;

            Expression<Func<TModel, TCollectionItem>> indexerLambda;
            {
                var expression = this.tableTemplate.Expression;
                var body = expression.Body;
                if (body.NodeType == ExpressionType.Convert)
                    body = ((UnaryExpression)body).Operand;

                if (body.Type.IsArray)
                {
                    var indexer = Expression.ArrayIndex(
                        body,
                        Expression.Constant(index));

                    indexerLambda = Expression.Lambda<Func<TModel, TCollectionItem>>(indexer, expression.Parameters);
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

                    indexerLambda = Expression.Lambda<Func<TModel, TCollectionItem>>(indexer, expression.Parameters);
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

            var viewTemplate = new TemplateArgs<TCollectionItem, RowInfo>(
                viewData,
                viewContext,
                new RowInfo(index, isNewRow));

            var row = new TableDataRowRenderer<TCollectionItem>(
                this.tableTemplate,
                this.tableTemplate.Columns,
                viewTemplate,
                index,
                indexHiddenFieldName,
                indexHiddenElementId);

            return row;
        }

        /// <summary>
        /// Renders a named section.
        /// </summary>
        /// <param name="sectionName">Name of the section to render.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the section to the output stream.</returns>
        public HelperResult RenderSection(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            return this.RenderSection(sectionName, true);
        }

        /// <summary>
        /// Renders a named section.
        /// </summary>
        /// <param name="sectionName">Name of the section to render.</param>
        /// <param name="required">A value indicating whether the section is required or not.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the section to the output stream.</returns>
        [ContractAnnotation("canbenull <= required: false; notnull <= required: true")]
        public HelperResult RenderSection(string sectionName, bool required)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            var helperResult = this.GetHelperResult(sectionName);

            if (helperResult == null && required)
                throw new Exception(string.Format("Section must be defined: {0}", sectionName));

            return helperResult;
        }

        /// <summary>
        /// Returns a value indicating whether the named section is defined or not.
        /// </summary>
        /// <param name="sectionName">Name of the section to test.</param>
        /// <returns>True if the section is defined; otherwise False.</returns>
        public bool IsSectionDefined(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            return this.tableTemplate.IsSectionDefined(sectionName, this.lazyViewTemplate.Value);
        }

        [CanBeNull]
        private HelperResult GetHelperResult([NotNull] string sectionName)
        {
            Debug.Assert(sectionName != null, "sectionName != null");
            if (!this.tableTemplate.IsSectionDefined(sectionName, this.lazyViewTemplate.Value))
                return null;

            var result = this.tableTemplate.GetSectionHelperResult(sectionName, this.lazyViewTemplate.Value);

            return result;
        }

        private class TableHeaderRowRendererCreator :
            ITableTemplateVisitor<TModel, TableHeaderRowRenderer>
        {
            private readonly HtmlHelper masterHtml;

            public TableHeaderRowRendererCreator(HtmlHelper masterHtml)
            {
                this.masterHtml = masterHtml;
            }

            [NotNull]
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
                        // Commented code: This code is commented to show what IS NOT NEEDED in this case,
                        // Commented code:     because this is a HEADER CELL.
                        ////TemplateInfo =
                        ////{
                        ////    HtmlFieldPrefix = this.html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                        ////        ExpressionHelper.GetExpressionText(value.Expression))
                        ////},
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

                    var viewTemplate = new TemplateArgs<TSubProperty>(viewData, viewContext);

                    var result = new TableHeaderCellRenderer<TSubProperty>(value, viewTemplate);
                    return result;
                }
            }
        }

        private readonly Lazy<TemplateArgs<IList<TCollectionItem>>> lazyViewTemplate;

        private TemplateArgs<IList<TCollectionItem>> CreateTemplateArgs()
        {
            Debug.Assert(this.tableTemplate.Expression != null, "this.tableTemplate.Expression != null");
            var getter = this.tableTemplate.Expression.Compile();
            var model = this.masterHtml.ViewData.Model;
            var collection = default(IList<TCollectionItem>);

            try
            {
                if (model != null)
                    collection = getter(model);
            }
            catch (NullReferenceException)
            {
            }

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

            var viewTemplate = new TemplateArgs<IList<TCollectionItem>>(viewData, viewContext);

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
        public HtmlHelper<IList<TCollectionItem>> Html
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

        /// <summary>
        /// Gets the <see cref="ModelMetadata"/> associated with this object.
        /// </summary>
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
        AjaxHelper IHelperContext.Ajax
        {
            get { return this.lazyViewTemplate.Value.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper IHelperContext.Html
        {
            get { return this.lazyViewTemplate.Value.Html; }
        }
    }
}