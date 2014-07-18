using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public interface ISectionRenderer
    {
        [NotNull]
        HelperResult RenderSection([NotNull] string sectionName);

        [ContractAnnotation("null <= required: false; notnull <= required: true")]
        HelperResult RenderSection([NotNull] string sectionName, bool required);

        bool IsSectionDefined([NotNull] string sectionName);
    }
}