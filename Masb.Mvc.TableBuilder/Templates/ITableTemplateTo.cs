using System.Collections.Generic;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableTemplateTo<TCollectionItem> :
        ITableTemplate
    {
        TResult Accept<TResult>(ITableTemplateToVisitor<TCollectionItem, TResult> visitor);

        new IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> Columns { get; }

        bool IsSectionDefined(string sectionName);

        HelperResult GetSectionHelperResult(string sectionName, IViewTemplate<IList<TCollectionItem>> viewTemplate);
    }
}