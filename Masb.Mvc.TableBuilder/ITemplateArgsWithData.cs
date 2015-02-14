using System;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents the `item` object that is passed to inline helpers.
    /// <para>Example:</para>
    /// <para><code>(<see cref="Func{T,TResult}"/>)@&lt;div>@item.Url.Action("Index")&lt;/div></code></para>
    /// </summary>
    /// <typeparam name="TInfo">Type of additional data provided to the rendering process.</typeparam>
    public interface ITemplateArgsWithData<out TInfo> :
        ITemplateArgs
    {
        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        TInfo Info { get; }
    }

    /// <summary>
    /// Represents the `item` object that is passed to inline helpers,
    /// when the inline helper renders model information to the page.
    /// <para>Example:</para>
    /// <para><code>(Func&lt;ITemplateArgs&lt;Person>, HelperResult>)@&lt;div>@item.Html.EditorFor(p => p.Name)&lt;/div></code></para>
    /// </summary>
    /// <typeparam name="TModel">Type of the model to render.</typeparam>
    /// <typeparam name="TInfo">Type of additional data provided to the rendering process.</typeparam>
    public interface ITemplateArgsWithData<TModel, out TInfo> :
        ITemplateArgsWithData<TInfo>,
        ITemplateArgs<TModel>
    {
    }
}