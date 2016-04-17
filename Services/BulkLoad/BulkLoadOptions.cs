using System.Collections.Generic;
using System.Data;
using Core.Interface;
using Core.Models.Options;

namespace Services.BulkLoad
{
    class BulkLoadOptions : BaseBulkLoad
    {
        private static readonly string[] ColumnNames = new string[]
            {
                "Symbol",  "ExperationDate", "CallPut", "Strike", "ContractName",
                "Last", "Bid", "Ask", "Change", "InTheMoney", "PercentChange",
                "Volume", "OpenInterest", "ImpliedVolatility", "DateCreated"
            };

        public BulkLoadOptions(ILogger logger)
            : base(logger, ColumnNames)
        {
        }

        public DataTable LoadDataTableWithOptions(IEnumerable<Options> dStats, DataTable dt)
        {
            foreach (var value in dStats)
            {
                string sValue = value.Symbol + "~" + value.ExperationDate + "~" + value.CallPut + "~"
                                + value.Strike + "~" + value.ContractName + "~" + value.Last + "~" 
                                + value.Bid + "~" + value.Ask + "~" + value.Change + "~" 
                                + value.InTheMoney + "~" + value.PercentChange + "~" + value.Volume
                                + "~" + value.OpenInterest + "~" + value.ImpliedVolatility + "~" + value.DateCreated;

                DataRow row = dt.NewRow();

                row.ItemArray = sValue.Split('~');

                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}
