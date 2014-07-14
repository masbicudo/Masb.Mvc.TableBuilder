using System.Collections.Generic;

namespace Masb.Mvc.TableBuilder
{
    public interface ITableDataRowRenderer
    {
        IEnumerable<ITableDataCellRenderer> Cells { get; }
    }
}