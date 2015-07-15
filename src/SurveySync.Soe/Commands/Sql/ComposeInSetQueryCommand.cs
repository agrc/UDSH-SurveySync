using System;
using System.Collections.Generic;
using System.Linq;
using CommandPattern;
using ESRI.ArcGIS.SOESupport;
using SurveySync.Soe.Extensions;

namespace SurveySync.Soe.Commands.Sql {

    public class ComposeInSetQueryCommand : Command<string> {
        private readonly bool _esriSql = true;
        private readonly string _field;
        private readonly string _items;
        private readonly string _returnFields;
        private readonly string _table;

        public ComposeInSetQueryCommand(string table, string field, IEnumerable<string> returnFields,
                                        IEnumerable<object> items) : this(field, items)
        {
            _table = table;
            _returnFields = string.Join(",", returnFields);
            _esriSql = false;
        }

        public ComposeInSetQueryCommand(string field, IEnumerable<object> items)
        {
            try
            {
                _items = string.Join(",", items.Select(x =>
                    {
                        if (x is string)
                        {
                            return "\"{0}\"".With(x);
                        }

                        return x.ToString();
                    }));
            }
            catch (NullReferenceException)
            {
#if !DEBUG
                Logger.LogMessage(ServerLogger.msgType.infoSimple, "ComposeInSetQueryCommand", 2472, "null reference exception on loop");
#endif
                _items = null;
            }

            _field = field;
        }

        /// <summary>
        ///     code to execute when command is run.
        /// </summary>
        public override void Execute()
        {
            if (string.IsNullOrEmpty(_items))
            {
                Result = null;
                return;
            }

            if (_esriSql)
            {
                Result = string.Format("{0} in ({1})", _field, _items);
                return;
            }

            Result = string.Format("select {0} from {1} where {2} in ({3})", _returnFields, _table, _field, _items);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "ComposeInSetQueryCommand";
        }
    }

}