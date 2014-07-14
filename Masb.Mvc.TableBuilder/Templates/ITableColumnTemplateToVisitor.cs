namespace Masb.Mvc.TableBuilder.Code
{
    public interface ITableColumnTemplateToVisitor<TSubProperty, out TResult>
    {
        TResult Visit<TCollectionItem>(ITableColumnTemplate<TCollectionItem, TSubProperty> value);
    }
}