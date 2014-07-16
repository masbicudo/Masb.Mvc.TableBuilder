using JetBrains.Annotations;
using System;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public static class SectionRendererExtensions
    {
        public static HelperResult RenderSection(
            [NotNull] this ISectionRenderer renderer,
            string sectionName,
            [NotNull] Func<object, HelperResult> defaultFunc)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            if (defaultFunc == null)
                throw new ArgumentNullException("defaultFunc");

            if (renderer.IsSectionDefined(sectionName))
                return renderer.RenderSection(sectionName);

            return defaultFunc(null);
        }
    }
}
