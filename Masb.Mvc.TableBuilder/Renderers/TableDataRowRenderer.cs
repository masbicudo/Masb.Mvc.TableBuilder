using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder
{
    public class TableDataRowRenderer<TCollectionItem> :
        IViewTemplate<TCollectionItem>,
        ITableDataRowRenderer
    {
        private readonly IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> columns;
        private readonly IViewTemplate<TCollectionItem> viewTemplate;
        private readonly int indexToRender;
        private readonly string indexHiddenFieldName;
        private readonly string indexHiddenElementId;

        public TableDataRowRenderer(
            IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> columns,
            IViewTemplate<TCollectionItem> viewTemplate,
            int indexToRender,
            string indexHiddenFieldName,
            string indexHiddenElementId)
        {
            this.columns = columns;
            this.viewTemplate = viewTemplate;
            this.indexToRender = indexToRender;
            this.indexHiddenFieldName = indexHiddenFieldName;
            this.indexHiddenElementId = indexHiddenElementId;
        }

        /// <summary>
        /// Gets the cells that compose this data row.
        /// </summary>
        public IEnumerable<ITableDataCellRenderer> Cells
        {
            get
            {
                var creator = new TableDataCellRendererCreator(this.viewTemplate.Html);
                var result = this.columns.Select(col => col.Accept(creator));
                return result;
            }
        }

        private class TableDataCellRendererCreator :
            ITableColumnTemplateFromVisitor<TCollectionItem, ITableDataCellRenderer>
        {
            private readonly HtmlHelper<TCollectionItem> html;

            public TableDataCellRendererCreator(HtmlHelper<TCollectionItem> html)
            {
                this.html = html;
            }

            public ITableDataCellRenderer Visit<TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value)
            {
                var rootModel = this.html.ViewData.Model;
                var modelGetter = value.Expression.Compile();
                TSubProperty model = default(TSubProperty);
                try
                {
                    model = modelGetter(rootModel);
                }
                catch (NullReferenceException)
                {
                }

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

                var result = new TableDataCellRenderer<TCollectionItem, TSubProperty>(value, viewTemplate);
                return result;
            }
        }

        /// <summary>
        /// Renders the hidden field responsible for indicating the existence of this index inside the collection,
        /// through the ".Index" property.
        /// </summary>
        /// <returns>An <see cref="MvcHtmlString"/> with the hidden field to render.</returns>
        public MvcHtmlString RenderIndexHiddenField()
        {
            return
                new MvcHtmlString(
                    string.Format(
                        @"<input type=""hidden"" id=""{0}"" name=""{1}"" value=""{2}"" />",
                        this.indexHiddenElementId,
                        this.indexHiddenFieldName,
                        this.indexToRender));
        }

        /// <summary>
        /// Renders the hidden field responsible for indicating the existence of this index inside the collection,
        /// through the ".Index" property.
        /// </summary>
        /// <param name="class"> The CSS class name that will be rendered for the hidden input field. </param>
        /// <returns> An <see cref="MvcHtmlString"/> with the hidden field to render. </returns>
        public MvcHtmlString RenderIndexHiddenField(string @class)
        {
            return
                new MvcHtmlString(
                    string.Format(
                        @"<input type=""hidden"" id=""{0}"" name=""{1}"" value=""{2}"" class=""{3}"" />",
                        this.indexHiddenElementId,
                        this.indexHiddenFieldName,
                        this.indexToRender,
                        @class));
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        public AjaxHelper<TCollectionItem> Ajax
        {
            get { return this.viewTemplate.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper<TCollectionItem> IViewTemplate<TCollectionItem>.Html
        {
            get { return this.viewTemplate.Html; }
        }

        /// <summary>
        /// Gets the current model object or value.
        /// </summary>
        public TCollectionItem Model
        {
            get { return this.viewTemplate.Model; }
        }

        public ModelMetadata Meta
        {
            get { return this.viewTemplate.Meta; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.UrlHelper"/> of the rendered snippet.
        /// </summary>
        public UrlHelper Url
        {
            get { return this.viewTemplate.Url; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        AjaxHelper IViewTemplate.Ajax
        {
            get { return this.viewTemplate.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper IViewTemplate.Html
        {
            get { return this.viewTemplate.Html; }
        }
    }
}