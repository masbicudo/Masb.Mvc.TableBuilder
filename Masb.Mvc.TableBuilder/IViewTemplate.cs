using System;
using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder.Code
{
    /// <summary>
    /// Represents the `item` object that is passed to inline helpers.
    /// <para>Example:</para>
    /// <para><code>(<see cref="Func&lt;IViewTemplate, HelperResult>"/>)@&lt;div>@item.Url.Action("Index")&lt;/div></code></para>
    /// </summary>
    public interface IViewTemplate
    {
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

    /// <summary>
    /// Represents the `item` object that is passed to inline helpers,
    /// when the inline helper renders model information to the page.
    /// <para>Example:</para>
    /// <para><code>(<see cref="Func&lt;IViewTemplate&lt;Person>, HelperResult>"/>)@&lt;div>@item.Html.EditorFor(p => p.Name)&lt;/div></code></para>
    /// </summary>
    /// <typeparam name="TModel">Type of the model to render.</typeparam>
    public interface IViewTemplate<TModel> :
        IViewTemplate
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
    }
}