namespace Masb.Mvc.TableBuilder.Code
{
    public interface ITableColumnTemplateFrom<TCollectionItem> :
        ITableColumnTemplate
    {
        TResult Accept<TResult>(ITableColumnTemplateFromVisitor<TCollectionItem, TResult> visitor);
    }
}