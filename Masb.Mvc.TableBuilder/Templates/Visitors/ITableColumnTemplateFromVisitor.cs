namespace Masb.Mvc.TableBuilder
{
    public interface ITableColumnTemplateFromVisitor<TCollectionItem, out TResult>
    {
        TResult Visit<TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value);
    }
}