using System.Collections.Generic;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableTemplateTo<TCollectionItem> :
        ITableTemplate
    {
        TResult Accept<TResult>(ITableTemplateToVisitor<TCollectionItem, TResult> visitor);

        new IEnumerable<ITableColumnTemplateFrom<TCollectionItem>> Columns { get; }
    }
}