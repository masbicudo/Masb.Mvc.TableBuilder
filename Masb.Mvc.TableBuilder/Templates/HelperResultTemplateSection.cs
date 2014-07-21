using System;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public class HelperResultTemplateSection<TInput> :
        ITemplateSection<TInput, HelperResult>
        where TInput : ITemplateArgs
    {
        [CanBeNull]
        private readonly Func<TInput, bool> predicate;

        [NotNull]
        private readonly Func<TInput, HelperResult> helper;

        public HelperResultTemplateSection([NotNull] Func<TInput, HelperResult> helper, [CanBeNull] Func<TInput, bool> predicate)
        {
            this.predicate = predicate;
            this.helper = helper;
        }

        /// <summary>
        /// Gets a value indicating whether this section can actually be rendered or not with the given input.
        /// </summary>
        /// <param name="input"> The input that is going to be used when rendering, if the test passes. </param>
        /// <returns>
        /// If True, indicates that the section can be rendered; otherwise the section cannot be rendered.
        /// </returns>
        public virtual bool CanRender(TInput input)
        {
            return this.predicate == null || this.predicate(input);
        }

        /// <summary>
        /// Renders the section with the given input.
        /// </summary>
        /// <param name="input"> The input that is going to be used. </param>
        /// <returns> Result representing the rendered section. </returns>
        public virtual HelperResult Render(TInput input)
        {
            return this.helper(input);
        }
    }
}