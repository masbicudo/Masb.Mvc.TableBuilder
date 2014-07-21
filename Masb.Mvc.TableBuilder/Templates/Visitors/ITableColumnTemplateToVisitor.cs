namespace Masb.Mvc.TableBuilder
{
    public interface ITableColumnTemplateToVisitor<TSubProperty, out TResult>
    {
        TResult Visit<TCollectionItem>(ITableColumnTemplate<TCollectionItem, TSubProperty> value);
    }
}