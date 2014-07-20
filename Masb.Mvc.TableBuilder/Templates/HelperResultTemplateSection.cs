using System;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public class HelperResultTemplateSection<TInput> :
        ITemplateSection<TInput, HelperResult>
        where TInput : ITemplateArgs
    {
        private readonly Func<TInput, bool> predicate;
        private readonly Func<TInput, HelperResult> helper;

        public HelperResultTemplateSection(Func<TInput, HelperResult> helper, Func<TInput, bool> predicate)
        {
            this.predicate = predicate;
            this.helper = helper;
        }

        public virtual bool CanRender(TInput input)
        {
            return this.predicate(input);
        }

        public virtual HelperResult Render(TInput input)
        {
            return this.helper(input);
        }
    }
}