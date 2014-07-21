namespace Masb.Mvc.TableBuilder
{
    public interface ITableColumnTemplateVisitor<out TResult>
    {
        TResult Visit<TCollectionItem, TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value);
    }
}