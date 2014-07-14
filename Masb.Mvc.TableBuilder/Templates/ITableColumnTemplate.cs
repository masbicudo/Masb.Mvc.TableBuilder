using System;
using System.Linq.Expressions;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableColumnTemplate
    {
        TResult Accept<TResult>(ITableColumnTemplateVisitor<TResult> visitor);
    }

    public interface ITableColumnTemplate<TCollectionItem, TSubProperty> :
        ITableColumnTemplateFrom<TCollectionItem>,
        ITableColumnTemplateTo<TSubProperty>
    {
        Expression<Func<TCollectionItem, TSubProperty>> Expression { get; }
    }
}