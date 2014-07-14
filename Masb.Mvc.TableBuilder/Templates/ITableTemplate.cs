using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableTemplate
    {
        IEnumerable<ITableColumnTemplate> Columns { get; }

        TResult Accept<TResult>(ITableTemplateVisitor<TResult> visitor);
    }

    public interface ITableTemplate<TModel> :
        ITableTemplate
    {
        TResult Accept<TResult>(ITableTemplateVisitor<TModel, TResult> visitor);
    }

    public interface ITableTemplate<TModel, TCollectionItem> :
        ITableTemplate<TModel>,
        ITableTemplateTo<TCollectionItem>
    {
        Expression<Func<TModel, IList<TCollectionItem>>> Expression { get; }
    }
}