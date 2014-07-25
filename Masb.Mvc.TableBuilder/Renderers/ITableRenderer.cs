using System.Collections.Generic;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents the ability to render tables.
    /// </summary>
    public interface ITableRenderer :
        IHelperContext,
        ISectionRenderer
    {
        /// <summary>
        /// Gets the header renderer.
        /// </summary>
        [NotNull]
        TableHeaderRowRenderer Header { get; }

        /// <summary>
        /// Gets a renderer for each item in the table.
        /// </summary>
        [NotNull]
        IEnumerable<ITableDataRowRenderer> Items { get; }

        /// <summary>
        /// Returns a renderer for a row that allows the insertion of new items.
        /// </summary>
        /// <param name="index">Index that is going to be considered as a new item.</param>
        /// <param name="defaultModel">Default model that will be used to render this new row.</param>
        /// <returns>A renderer that helps rendering the "new line".</returns>
        [NotNull]
        ITableDataRowRenderer NewItem(int index, object defaultModel);

        /// <summary>
        /// Returns a renderer for a row that allows the insertion of new items.
        /// </summary>
        /// <param name="index">Index that is going to be considered as a new item.</param>
        /// <returns>A renderer that helps rendering the "new line".</returns>
        [NotNull]
        ITableDataRowRenderer NewItem(int index);
    }
}