using System.Collections.Generic;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableTemplateTo<TCollectionItem> :
        ITableTemplate
    {
        TResult Accept<TResult>(ITableTemplateToVisitor<TCollectionItem, TResult> visitor);

        new IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> Columns { get; }

        bool IsSectionDefined([NotNull] string sectionName, IViewTemplate<IList<TCollectionItem>> viewTemplate);

        [CanBeNull]
        HelperResult GetSectionHelperResult([NotNull] string sectionName, IViewTemplate<IList<TCollectionItem>> viewTemplate);

        bool IsItemSectionDefined([NotNull] string sectionName, IViewTemplateWithData<TCollectionItem, RowInfo> viewTemplate);

        [CanBeNull]
        HelperResult GetItemSectionHelperResult([NotNull] string sectionName, IViewTemplateWithData<TCollectionItem, RowInfo> viewTemplate);
    }
}