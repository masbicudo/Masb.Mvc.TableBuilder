namespace Masb.Mvc.TableBuilder.Code
{
    public interface ITableColumnTemplateVisitor<out TResult>
    {
        TResult Visit<TCollectionItem, TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value);
    }
}