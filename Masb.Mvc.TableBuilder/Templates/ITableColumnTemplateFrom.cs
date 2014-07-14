namespace Masb.Mvc.TableBuilder
{
    public interface ITableColumnTemplateFrom<TCollectionItem> :
        ITableColumnTemplate
    {
        TResult Accept<TResult>(ITableColumnTemplateFromVisitor<TCollectionItem, TResult> visitor);
    }
}