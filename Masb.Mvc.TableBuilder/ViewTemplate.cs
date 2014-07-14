using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder.Code
{
    public class ViewTemplate<TModel> :
        IViewTemplate<TModel>,
        IViewDataContainer
    {
        public ViewTemplate(ViewDataDictionary<TModel> viewData, ViewContext viewContext)
        {
            this.ViewData = viewData;
            this.Meta = viewData.ModelMetadata;
            this.Url = new UrlHelper(viewContext.RequestContext);
            this.Html = new HtmlHelper<TModel>(viewContext, this);
            this.Ajax = new AjaxHelper<TModel>(viewContext, this);
        }

        public ModelMetadata Meta { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.UrlHelper"/> of the rendered snippet.
        /// </summary>
        public UrlHelper Url { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        public AjaxHelper<TModel> Ajax { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        AjaxHelper IViewTemplate.Ajax
        {
            get { return this.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        public HtmlHelper<TModel> Html { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper IViewTemplate.Html
        {
            get { return this.Html; }
        }

        /// <summary>
        /// Gets or sets the view data dictionary.
        /// </summary>
        /// <returns>
        /// The view data dictionary.
        /// </returns>
        public ViewDataDictionary ViewData { get; set; }
    }
}