namespace Masb.Mvc.TableBuilder
{
    public interface ITableTemplateVisitor<out TResult>
    {
        TResult Visit<TModel, TCollectionItem>(ITableTemplate<TModel, TCollectionItem> value);
    }

    public interface ITableTemplateVisitor<TModel, out TResult>
    {
        TResult Visit<TCollectionItem>(ITableTemplate<TModel, TCollectionItem> value);
    }
}