namespace Masb.Mvc.TableBuilder
{
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

        public int RowIndex
        {
            get { return this.rowIndex; }
        }

        public bool IsNewRow
        {
            get { return isNewRow; }
        }
    }
}