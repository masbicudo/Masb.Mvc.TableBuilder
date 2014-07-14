using System.Web.Mvc;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public class TableDataCellRenderer<TCollectionItem, TSubProperty> :
        ITableDataCellRenderer,
        IViewTemplate<TSubProperty>
    {
        private readonly ITableColumnTemplate<TCollectionItem, TSubProperty> tableColumnTemplate;
        private readonly IViewTemplate<TSubProperty> viewTemplate;

        public TableDataCellRenderer(ITableColumnTemplate<TCollectionItem, TSubProperty> tableColumnTemplate, IViewTemplate<TSubProperty> viewTemplate)
        {
            this.tableColumnTemplate = tableColumnTemplate;
            this.viewTemplate = viewTemplate;
        }

        public HelperResult Render()
        {
            var result = this.tableColumnTemplate.GetDataHelperResult(this.viewTemplate);
            return result;
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        public AjaxHelper<TSubProperty> Ajax
        {
            get { return this.viewTemplate.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper<TSubProperty> IViewTemplate<TSubProperty>.Html
        {
            get { return this.viewTemplate.Html; }
        }

        /// <summary>
        /// Gets the current model object or value.
        /// </summary>
        public TSubProperty Model
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