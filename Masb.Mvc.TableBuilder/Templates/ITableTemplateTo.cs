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

        bool IsSectionDefined([NotNull] string sectionName, ITemplateArgs<IList<TCollectionItem>> templateArgs);

        [CanBeNull]
        HelperResult GetSectionHelperResult([NotNull] string sectionName, ITemplateArgs<IList<TCollectionItem>> templateArgs);

        bool IsItemSectionDefined([NotNull] string sectionName, ITemplateArgsWithData<TCollectionItem, RowInfo> templateArgs);

        [CanBeNull]
        HelperResult GetItemSectionHelperResult([NotNull] string sectionName, ITemplateArgsWithData<TCollectionItem, RowInfo> templateArgs);
    }
}