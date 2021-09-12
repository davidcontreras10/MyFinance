using System.Collections.Generic;

namespace MyFinanceWebApp.Models
{
    public class BasicTable
    {
        #region Constructor

        public BasicTable()
        {
            Rows = new List<BasicTableRow>();
        }

        #endregion

        #region Attributes

        public BasicTableCell Title { set; get; }
        public BasicTableRow Header { set; get; }
        public List<BasicTableRow> Rows { set; get; } 

        #endregion
    }

    public class BasicTableRow
    {
        #region Constructor

        public BasicTableRow()
        {
            Cells = new List<BasicTableCell>();
        }

        #endregion

        #region Attributes

        public List<BasicTableCell> Cells { set; get; }
        public object RowId { set; get; }
        public string InLineAttributes { get; set; }

        #endregion
    }

    public class BasicTableCell
    {

        #region Attributes

        public object Value { set; get; }

        #endregion
    }
}
