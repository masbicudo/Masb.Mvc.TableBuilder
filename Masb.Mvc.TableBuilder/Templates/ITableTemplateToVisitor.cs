namespace Masb.Mvc.TableBuilder
{
    public interface ITableTemplateToVisitor<TCollectionItem, out TResult>
    {
        TResult Visit<TModel>(ITableTemplate<TModel, TCollectionItem> value);
    }
}