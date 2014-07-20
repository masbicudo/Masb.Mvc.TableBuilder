namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a section of a template.
    /// </summary>
    /// <typeparam name="TInput">Type of input that will be passed to the template when rendering.</typeparam>
    /// <typeparam name="TOutput">The type that results from this section.</typeparam>
    public interface ITemplateSection<in TInput, out TOutput>
        where TInput : ITemplateArgs
    {
        /// <summary>
        /// Gets a value indicating whether this section can actually be rendered or not with the given input.
        /// </summary>
        /// <param name="input"> The input that is going to be used when rendering, if the test passes. </param>
        /// <returns>
        /// If True, indicates that the section can be rendered; otherwise the section cannot be rendered.
        /// </returns>
        bool CanRender(TInput input);

        /// <summary>
        /// Renders the section with the given input.
        /// </summary>
        /// <param name="input"> The input that is going to be used. </param>
        /// <returns> Result representing the rendered section. </returns>
        TOutput Render(TInput input);
    }
}