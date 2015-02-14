using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents the `item` object that is passed to inline helpers,
    /// when the inline helper renders model information to the page.
    /// <para>Example:</para>
    /// <para><code>(Func&lt;ITemplateArgs&lt;Person>, HelperResult>)@&lt;div>@item.Html.EditorFor(p => p.Name)&lt;/div></code></para>
    /// </summary>
    /// <typeparam name="TModel">Type of the model to render.</typeparam>
    /// <typeparam name="TInfo">Type of additional data provided to the rendering process.</typeparam>
    public class TemplateArgs<TModel, TInfo> : TemplateArgs<TModel>,
        ITemplateArgsWithData<TModel, TInfo>
    {
        public TemplateArgs(ViewDataDictionary<TModel> viewData, ViewContext viewContext, TInfo info)
            : base(viewData, viewContext)
        {
            this.Info = info;
        }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        public TInfo Info { get; private set; }
    }

    /// <summary>
    /// Represents the `item` object that is passed to inline helpers,
    /// when the inline helper renders model information to the page.
    /// <para>Example:</para>
    /// <para><code>(Func&lt;ITemplateArgs&lt;Person>, HelperResult>)@&lt;div>@item.Html.EditorFor(p => p.Name)&lt;/div></code></para>
    /// </summary>
    /// <typeparam name="TModel">Type of the model to render.</typeparam>
    public class TemplateArgs<TModel> :
        ITemplateArgs<TModel>,
        IViewDataContainer
    {
        public TemplateArgs(ViewDataDictionary<TModel> viewData, ViewContext viewContext)
        {
            this.ViewData = viewData;
            this.Meta = viewData.ModelMetadata;
            this.Url = new UrlHelper(viewContext.RequestContext);
            this.Html = new HtmlHelper<TModel>(viewContext, this);
            this.Ajax = new AjaxHelper<TModel>(viewContext, this);
        }

        /// <summary>
        /// Gets the <see cref="ModelMetadata"/> associated with this object.
        /// </summary>
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
        AjaxHelper IHelperContext.Ajax
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
        /// Gets the current model object or value.
        /// </summary>
        public TModel Model
        {
            get { return this.Html.ViewData.Model; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper IHelperContext.Html
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