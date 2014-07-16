using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public interface ISectionRenderer
    {
        [NotNull]
        HelperResult RenderSection([NotNull] string sectionName);

        [CanBeNull]
        HelperResult RenderSection([NotNull] string sectionName, bool required);

        bool IsSectionDefined([NotNull] string sectionName);
    }
}