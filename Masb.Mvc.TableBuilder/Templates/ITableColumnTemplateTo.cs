using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableColumnTemplateTo<TSubProperty> :
        ITableColumnTemplate
    {
        HelperResult GetHeaderHelperResult(ITemplateArgs<TSubProperty> templateArgs);

        HelperResult GetDataHelperResult(ITemplateArgs<TSubProperty> templateArgs);

        bool IsSectionDefined(string sectionName);

        HelperResult GetSectionHelperResult(string sectionName, ITemplateArgs<TSubProperty> templateArgs);

        TResult Accept<TResult>(ITableColumnTemplateToVisitor<TSubProperty, TResult> visitor);
    }
}