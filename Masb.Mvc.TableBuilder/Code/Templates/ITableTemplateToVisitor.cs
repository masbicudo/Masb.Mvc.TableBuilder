namespace Masb.Mvc.TableBuilder.Code
{
    public interface ITableTemplateToVisitor<TCollectionItem, out TResult>
    {
        TResult Visit<TModel>(ITableTemplate<TModel, TCollectionItem> value);
    }
}