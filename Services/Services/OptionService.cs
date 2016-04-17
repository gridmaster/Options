using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Business;
using Core.Interface;
using Core.Models;
using Core.Models.Options;
using DIContainer;
using Services.BulkLoad;
using Services.Interfaces;
using Services.SQL;

namespace Services.Services
{
// ReSharper disable SuggestUseVarKeywordEvident
    public class OptionService : BaseService, IOptionService
    {
        #region Constructors

        public OptionService(ILogger logger)
            : base(logger)
        {
            ThrowIfIsInitialized();
            IsInitialized = true;
        }

        #endregion Constructors

        #region IOptionService Implementation

        public bool GetOptions()
        {
            bool success = false;

            // this is for debugging and testing
            //bool start = true;
            //string startSymbol = "AAP";

            IOCContainer.Instance.Get<ILogger>().InfoFormat("{0}GetOptions's runnin...{0}", Environment.NewLine);

            OptionList options = new OptionList();
            options.Options = new List<Options>();
            Dictionary<String, String> optDates = new Dictionary<string, string>(); 
            optDates = GetDates();

            List<String> symbols = SymbolContext.GetSymbolList();

            foreach (string sym in symbols)
            {

                //if (sym != startSymbol && !start)
                //{
                //    continue;
                //}
                //else
                //{
                //    start = true;
                //}

                if (sym.IndexOf("^") > -1) continue;

                List<String> callRows = new List<String>();
                List<String> putRows = new List<String>();
                var myDates = optDates.ToArray();

                try
                {
                    int count = 0;
                    for (int i = 0; i < myDates.Length; i++)
                    {
                        do
                        {
                            string webPage = GetWebPage(string.Format(OptionUris.UriOptionWithDate, sym, myDates[i].Key));

                            if (String.IsNullOrEmpty(webPage)) continue;

                            if (optDates == null)
                            {
                                count++;
                                continue;
                            }

                            if (optDates.Count == 0 && count < 10)
                            {
                                count++;
                                continue;
                            }

                            callRows.AddRange(GetOptionRows(ref webPage, "Calls", myDates[i].Value));
                            putRows.AddRange(GetOptionRows(ref webPage, "Puts", myDates[i].Value));
                            if (callRows.Count == 0 && putRows.Count == 0) count++;
                        } while (callRows.Count == 0 && putRows.Count == 0 && count < 3);
                    }
                }
                catch (Exception ex)
                {
                    IOCContainer.Instance.Get<ILogger>().InfoFormat("{0}GetOptions's GetWebPage Error: {0}{1}", ex.Message,
                        Environment.NewLine);
                }

                if (optDates == null) continue;

                //for (int iNdx = 0; iNdx < optDates.Count; iNdx++)
                //{
                    for (int i = 0; i < callRows.Count; i++)
                    {
                        string exparationDate = callRows[i].Substring(0, callRows[i].IndexOf(" -"));
                        try
                        {
                            Options opt = new Options()
                                {
                                    Symbol = sym,
                                    CallPut = "Call",
                                    DateCreated = DateTime.Now,
                                    ExperationDate = System.Convert.ToDateTime(exparationDate),
                                };
                            opt = LoadRow(opt, callRows[i]);
                            options.Options.Add(opt);
                        }
                        catch (Exception ex)
                        {
                            IOCContainer.Instance.Get<ILogger>()
                                        .InfoFormat("{0}GetOptions's Call Error: {0}{1}", ex.Message,
                                                    Environment.NewLine);
                        }
                    }

                    for (int i = 0; i < putRows.Count; i++)
                    {
                        string exparationDate = putRows[i].Substring(0, putRows[i].IndexOf(" -"));
                        try
                        {
                            Options opt = new Options()
                                {
                                    Symbol = sym,
                                    CallPut = "Put",
                                    DateCreated = DateTime.Now,
                                    ExperationDate = System.Convert.ToDateTime(exparationDate),
                                };
                            opt = LoadRow(opt, putRows[i]);
                            options.Options.Add(opt);
                        }
                        catch (Exception ex)
                        {
                            IOCContainer.Instance.Get<ILogger>()
                                        .InfoFormat("{0}GetOptions's Put Error: {0}{1}", ex.Message, Environment.NewLine);
                        }
                    }
               // }
                try
                {
                    var dt = IOCContainer.Instance.Get<BulkLoadOptions>().ConfigureDataTable();

                    dt = IOCContainer.Instance.Get<BulkLoadOptions>().LoadDataTableWithOptions(options.Options, dt);

                    if (dt == null)
                    {
                        IOCContainer.Instance.Get<ILogger>()
                                    .InfoFormat("{0}No data returned on LoadDataTableWithOptions", Environment.NewLine);
                    }
                    else
                    {
                        success = IOCContainer.Instance.Get<BulkLoadOptions>().BulkCopy<Options>(dt, "OptionContext");
                        IOCContainer.Instance.Get<ILogger>()
                                    .InfoFormat("{0}BulkLoadOptions returned with: {1}", Environment.NewLine,
                                                success ? "Success" : "Fail");
                    }
                }
                catch (Exception ex)
                {
                    IOCContainer.Instance.Get<ILogger>().InfoFormat("{0}Bulk Load Options Error: {1}", Environment.NewLine, ex.Message);
                }
                finally
                {
                    options.Options = new List<Options>();
                }
            }

            return true;
        }

        #endregion IOptionService Implementation

        private Options LoadRow(Options option, string data)
        {
            try
            {
                option.InTheMoney = data.IndexOf("class=\"in-the-money", System.StringComparison.Ordinal) > -1 ? true : false;

                string strike =
                    data.Substring(data.IndexOf("strike=", System.StringComparison.Ordinal) + "strike=".Length);
                decimal value = 0.00M;
                string number = strike.Substring(0, strike.IndexOf("\"", System.StringComparison.Ordinal));
                bool result = decimal.TryParse(number, out value);
                option.Strike = value;
                data = data.Substring(data.IndexOf("q?s=", System.StringComparison.Ordinal) + "q?s=".Length);
                option.ContractName = data.Substring(0, data.IndexOf("\"", System.StringComparison.Ordinal));

                option.Last = GetDecimal(ref data);
                option.Bid = GetDecimal(ref data);
                option.Ask = GetDecimal(ref data);
                option.Change = GetDecimal(ref data);

                option.PercentChange = GetPercent(ref data);
                option.Volume = GetVolume(ref data);
                option.OpenInterest = GetDecimal(ref data);
                option.ImpliedVolatility = GetPercent(ref data);
            }
            catch (Exception ex)
            {
                IOCContainer.Instance.Get<ILogger>().InfoFormat("{0}LoadRow Error: {0}{1}", ex.Message, Environment.NewLine);
            }
            return option;
        }

        private decimal GetVolume(ref string data)
        {
            bool result = false;
            decimal value = 0.00M;

            try
            {
                data =
                    data.Substring(data.IndexOf("volume", System.StringComparison.Ordinal) + "option_entry Fz-m".Length);
                data = data.Substring(data.IndexOf(">", System.StringComparison.Ordinal) + 1);
                string number = data.Substring(0, data.IndexOf("<", System.StringComparison.Ordinal));
                result = decimal.TryParse(number, out value);

            }
            catch (Exception ex)
            {
                IOCContainer.Instance.Get<ILogger>()
                            .InfoFormat("{0}GetDecimal Error: {0}{1}", ex.Message, Environment.NewLine);
            }

            return result ? value : 0.00m;
        }

        private decimal GetDecimal(ref string data)
        {
            bool result = false;
            decimal value = 0.00M;

            try
            {
                data =
                    data.Substring(data.IndexOf("option_entry Fz-m", System.StringComparison.Ordinal) +
                                   "option_entry Fz-m".Length);
                data = data.Substring(data.IndexOf(">", System.StringComparison.Ordinal) + 1);
                string number = data.Substring(0, data.IndexOf("<", System.StringComparison.Ordinal));
                result = decimal.TryParse(number, out value);
            }
            catch (Exception ex)
            {
                IOCContainer.Instance.Get<ILogger>().InfoFormat("{0}GetDecimal Error: {0}{1}", ex.Message, Environment.NewLine);
            }

            return result ? value : 0.00m;
        }

        private string GetPercent(ref string data)
        {
            string number = String.Empty;
            try
            {
                data =
                    data.Substring(data.IndexOf("option_entry Fz-m", System.StringComparison.Ordinal) +
                                   "option_entry Fz-m".Length);
                data = data.Substring(data.IndexOf(">", System.StringComparison.Ordinal) + 1);
                number = data.Substring(0, data.IndexOf("<", System.StringComparison.Ordinal));
            }
            catch (Exception ex)
            {
                IOCContainer.Instance.Get<ILogger>()
                            .InfoFormat("{0}GetDecimal Error: {0}{1}", ex.Message, Environment.NewLine);
            }

            return number;
        }

        private List<String> GetOptionRows(ref string options, string callput, string optionDate)
        {
            List<String> optionRows = new List<String>();

            try
            {
                //string pattern = "<caption>.*\r*\n*" + callput + "\r*\n*.*</caption>";
                //Regex mainReg = new Regex(pattern);
                //var stuff = mainReg.Match(options);
                String calls = "";
                string table = options.Substring(options.IndexOf("<caption>", System.StringComparison.Ordinal));

                if (callput == "Calls")
                {                    
                    calls = table.Substring(1, table.IndexOf("</table>", System.StringComparison.Ordinal));
                }
                else {
                    table = table.Substring(table.IndexOf("</table>"));
                    table = table.Substring(table.IndexOf("<caption>"));
                    calls = table.Substring(1, table.IndexOf("</table>", System.StringComparison.Ordinal));
                }

                Regex regex = new Regex("tr data-row=");
                
                int TableLength = calls.Length + "</table>".Length;
                var match = regex.Match(calls);

                if (match.Index == 0)
                {
                    match = regex.Match(options);
                    if (match.Index == 0)
                    {
                        return optionRows;
                    }
                    else
                    {
                        calls = options;
                    }
                }

                do
                {
                    calls = calls.Substring(match.Index - 1);
                    int ndx = calls.IndexOf("</tr>", System.StringComparison.Ordinal);
                    optionRows.Add(optionDate + " - " + calls.Substring(0, ndx + "</tr>".Length));
                    calls = calls.Substring(ndx);
                    match = regex.Match(calls);
                } while (match.Length > 0);

                //options = options.Substring(TableLength);
            }
            catch (Exception ex)
            {
                IOCContainer.Instance.Get<ILogger>()
                            .InfoFormat("{0}GetOptionRows Error: {0}{1}", ex.Message, Environment.NewLine);
            }

            return optionRows;
        }

        private Dictionary<String, String> GetDates()
        {
            Dictionary<String, String> optDates = new Dictionary<string, string>();

            try
            {
                DateTime nextFriday = GetNextWeekday(DateTime.Today, DayOfWeek.Friday);
                DateTime friday = nextFriday.Date;
                int month = nextFriday.Month;
                int year = nextFriday.Year;
                long unixTime = 0L;
                for (int i = 0; i < 8; i++)
                {
                    unixTime = UnixTime.ToUnixTime(friday.AddDays(i * 7));
                    optDates.Add(unixTime.ToString(), nextFriday.AddDays(i * 7).ToString());
                }
                var lastDate = optDates.Values.Last();
                DateTime.TryParse(lastDate, out nextFriday);
                if (nextFriday.Day < 15)
                {
                    month = nextFriday.Month + 1;
                    year = nextFriday.Year;
                    nextFriday = nextFriday.AddDays(15 - nextFriday.Day);
                    nextFriday = GetNextWeekday(nextFriday, DayOfWeek.Friday);
                }
                else
                {
                    nextFriday = nextFriday.AddDays(-(nextFriday.Day-15));
                    nextFriday = nextFriday.AddMonths(1);
                }

                unixTime = UnixTime.ToUnixTime(nextFriday);
                optDates.Add(unixTime.ToString(), nextFriday.ToString());
                for (int i = 0; i < 4; i++)
                {
                    nextFriday = nextFriday.AddDays(-(nextFriday.Day - 15));
                    nextFriday = nextFriday.AddMonths(1);
                    nextFriday = GetNextWeekday(nextFriday, DayOfWeek.Friday);
                    unixTime = UnixTime.ToUnixTime(nextFriday);
                    optDates.Add(unixTime.ToString(), nextFriday.ToString());
                }
                for (int i = 0; i < 2; i++)
                {
                    nextFriday = nextFriday.AddDays(-(nextFriday.Day - 15));
                    nextFriday = nextFriday.AddMonths(-nextFriday.Month + 1);
                    nextFriday = nextFriday.AddYears(1);
                    nextFriday = GetNextWeekday(nextFriday, DayOfWeek.Friday);
                    unixTime = UnixTime.ToUnixTime(nextFriday);
                    optDates.Add(unixTime.ToString(), nextFriday.ToString());
                }
            }
            catch (Exception ex)
            {
                IOCContainer.Instance.Get<ILogger>()
                            .InfoFormat("{0}GetOptionRows Error: {0}{1}", ex.Message, Environment.NewLine);
            }

            return optDates;
        }
        
        public DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
        
        private Dictionary<String, String> GetDates(ref string options)
        {
            Dictionary<String, String> optDates = new Dictionary<string, string>();

            try
            {
                Regex regex = new Regex("<b class='SelectBox-Text '>.*</b>");

                var match = regex.Match(options);

                if (match.Length == 0) return null;

                regex = new Regex("<b class='SelectBox-Text '>");
                match = regex.Match(options);
                options = options.Substring(match.Index + match.Length);


                regex = new Regex("<option data-selectbox-link=");
                match = regex.Match(options);
                do
                {
                    options = options.Substring(match.Index + match.Length);
                    int ndx = options.IndexOf(">", System.StringComparison.Ordinal);
                    int ndx2 = options.IndexOf("value=", System.StringComparison.Ordinal) + "value=".Length;
                    if (options.IndexOf("<", System.StringComparison.Ordinal) - ndx - 1 < 0) return null;
                    optDates.Add(options.Substring(ndx2, ndx - ndx2),
                                 options.Substring(ndx + 1,
                                                   options.IndexOf("<", System.StringComparison.Ordinal) - ndx - 1));
                    match = regex.Match(options);
                } while (match.Length > 0);

            }
            catch (Exception ex)
            {
                IOCContainer.Instance.Get<ILogger>()
                            .InfoFormat("{0}GetOptionRows Error: {0}{1}", ex.Message, Environment.NewLine);
            }

            return optDates;
        }

        private List<Options> LoadOptions(string uri)
        {
            return new List<Options>();
        }
    }

    // ReSharper restore SuggestUseVarKeywordEvident
}