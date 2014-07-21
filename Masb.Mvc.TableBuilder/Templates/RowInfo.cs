namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents information of the row being rendered.
    /// </summary>
    public struct RowInfo
    {
        private readonly int rowIndex;
        private readonly bool isNewRow;

        public RowInfo(int rowIndex, bool isNewRow)
            : this()
        {
            this.rowIndex = rowIndex;
            this.isNewRow = isNewRow;
        }

        /// <summary>
        /// Gets a value indicating the index of the current row in the collection.
        /// </summary>
        public int RowIndex
        {
            get { return this.rowIndex; }
        }

        /// <summary>
        /// Gets a value indicating whether this row represents an "insertion row".
        /// </summary>
        public bool IsNewRow
        {
            get { return this.isNewRow; }
        }
    }
}