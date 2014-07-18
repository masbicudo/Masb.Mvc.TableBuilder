using System.Collections.Generic;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableRenderer :
        IViewTemplate,
        ISectionRenderer
    {
        TableHeaderRowRenderer Header { get; }

        IEnumerable<ITableDataRowRenderer> Items { get; }

        ITableDataRowRenderer NewItem(int index, object defaultModel);

        ITableDataRowRenderer NewItem(int index);
    }
}