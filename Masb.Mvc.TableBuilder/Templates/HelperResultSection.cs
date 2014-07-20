using System;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public class HelperResultSection<TInput> :
        ISection<TInput, HelperResult>
        where TInput : IViewTemplate
    {
        private readonly Func<TInput, bool> predicate;
        private readonly Func<TInput, HelperResult> helper;

        public HelperResultSection(Func<TInput, HelperResult> helper, Func<TInput, bool> predicate)
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