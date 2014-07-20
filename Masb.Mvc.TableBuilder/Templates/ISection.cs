namespace Masb.Mvc.TableBuilder
{
    public interface ISection<in TInput, out TOutput>
        where TInput : IViewTemplate
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