using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nvelope;

namespace Nvelope.Tabular
{
    public struct CellLoc : IEquatable<CellLoc>
    {
        public CellLoc(int row, int col)
            : this()
        {
            Row = row;
            Col = col;
        }

        public CellLoc(string cellCoords) : this()
        {
            var parts = Regex.Match(cellCoords, "^([A-Za-z]+)([0-9]+)$");
            if (!parts.Success)
                throw new FormatException("The value '" + cellCoords + "' was not a valid cell coordinate. Expected something like 'AK343'");
            var colPart = parts.Groups[1].Value.ToUpper();
            var colNum = 'A'.Inc('Z').IndexOfEach(colPart).Single();
            var rowNum = parts.Groups[2].Value.ConvertTo<int>() - 1;

            Row = rowNum;
            Col = colNum;
        }

        public readonly int Row;
        public readonly int Col;

        public override string ToString()
        {
            return "[" + Row + "," + Col + "]";
        }

        public static bool operator ==(CellLoc a, CellLoc b)
        {
            return a.CompareTo(b) == 0;
        }

        public static bool operator !=(CellLoc a, CellLoc b)
        {
            return a.CompareTo(b) != 0;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is CellLoc)
                return this.CompareTo((CellLoc)obj);
            return -1;
        }

        #endregion

        #region IComparable<CellLoc> Members

        public int CompareTo(CellLoc other)
        {
            if (this.Row != other.Row)
                return this.Row - other.Row;
            if (this.Col != other.Col)
                return this.Col - other.Col;
            return 0;
        }

        #endregion

        #region IEquatable<CellLoc> Members
        
        public override int GetHashCode()
        {
            return this.Row ^ this.Col;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CellLoc))
                return false;

            return Equals((CellLoc)obj);
        }

        public bool Equals(CellLoc other)
        {
            return this.CompareTo(other) == 0;
        }
        #endregion

    }

    public static class CellLocExtensions
    {
        public static IEnumerable<CellLoc> ColRange(this CellLoc cell, int length)
        {
            return cell.Col.To(cell.Col + length).Chop().Select(c => new CellLoc(cell.Row, c));
        }

        public static IEnumerable<CellLoc> RowRange(this CellLoc cell, int length)
        {
            return cell.Row.To(cell.Row + length).Chop().Select(r => new CellLoc(r, cell.Col));
        }

        public static CellLoc Below(this CellLoc cell, int offset = 1)
        {
            return new CellLoc(cell.Row + offset, cell.Col);
        }

        public static CellLoc Above(this CellLoc cell, int offset = 1)
        {
            return new CellLoc(cell.Row - offset, cell.Col);
        }

        public static CellLoc Left(this CellLoc cell, int offset = 1)
        {
            return new CellLoc(cell.Row, cell.Col - offset);
        }

        public static CellLoc Right(this CellLoc cell, int offset = 1)
        {
            return new CellLoc(cell.Row, cell.Col + offset);
        }

        public static IEnumerable<CellLoc> To(this CellLoc cell, CellLoc other)
        {
            foreach (var row in cell.Row.To(other.Row))
                foreach (var col in cell.Col.To(other.Col))
                    yield return new CellLoc(row, col);
        }

        public static CellLoc AtCol(this CellLoc cell, int col)
        {
            return new CellLoc(cell.Row, col);
        }

        public static CellLoc AtRow(this CellLoc cell, int row)
        {
            return new CellLoc(row, cell.Col);
        }
    
    }
}
