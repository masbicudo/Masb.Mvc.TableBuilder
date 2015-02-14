namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents the `item` object that is passed to inline helpers.
    /// <para>Example:</para>
    /// <para><code>(Func&lt;ITemplateArgs, HelperResult>)@&lt;div>@item.Url.Action("Index")&lt;/div></code></para>
    /// </summary>
    public interface ITemplateArgs :
        IHelperContext
    {
    }

    /// <summary>
    /// Represents the `item` object that is passed to inline helpers,
    /// when the inline helper renders model information to the page.
    /// <para>Example:</para>
    /// <para><code>(Func&lt;ITemplateArgs&lt;Person>, HelperResult>)@&lt;div>@item.Html.EditorFor(p => p.Name)&lt;/div></code></para>
    /// </summary>
    /// <typeparam name="TModel">Type of the model to render.</typeparam>
    public interface ITemplateArgs<TModel> :
        IHelperContext<TModel>,
        ITemplateArgs
    {
    }
}