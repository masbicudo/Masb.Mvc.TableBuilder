namespace Masb.Mvc.TableBuilder.Code
{
    public interface ITableColumnTemplateFromVisitor<TCollectionItem, out TResult>
    {
        TResult Visit<TSubProperty>(ITableColumnTemplate<TCollectionItem, TSubProperty> value);
    }
}