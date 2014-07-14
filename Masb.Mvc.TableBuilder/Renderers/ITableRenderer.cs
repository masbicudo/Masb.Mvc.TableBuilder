using System.Collections.Generic;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableRenderer
    {
        TableHeaderRowRenderer Header { get; }

        IEnumerable<ITableDataRowRenderer> Items { get; }
    }
}