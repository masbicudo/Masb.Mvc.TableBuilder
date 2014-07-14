using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableColumnTemplateTo<TSubProperty> :
        ITableColumnTemplate
    {
        HelperResult GetHeaderHelperResult(IViewTemplate<TSubProperty> viewTemplate);

        HelperResult GetDataHelperResult(IViewTemplate<TSubProperty> viewTemplate);

        TResult Accept<TResult>(ITableColumnTemplateToVisitor<TSubProperty, TResult> visitor);
    }
}