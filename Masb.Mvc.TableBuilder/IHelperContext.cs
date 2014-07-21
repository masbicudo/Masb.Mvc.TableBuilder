using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a context in which a helper can render with a model.
    /// </summary>
    /// <typeparam name="TModel">Type of the model to render.</typeparam>
    public interface IHelperContext<TModel> :
        IHelperContext
    {
        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        new AjaxHelper<TModel> Ajax { get; }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        new HtmlHelper<TModel> Html { get; }

        /// <summary>
        /// Gets the model object or value in the current helper context.
        /// </summary>
        TModel Model { get; }
    }

    /// <summary>
    /// Represents a context in which a helper can render.
    /// </summary>
    public interface IHelperContext
    {
        /// <summary>
        /// Gets the <see cref="ModelMetadata"/> associated with this object.
        /// </summary>
        ModelMetadata Meta { get; }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.UrlHelper"/> of the rendered snippet.
        /// </summary>
        UrlHelper Url { get; }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        AjaxHelper Ajax { get; }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper Html { get; }
    }
}