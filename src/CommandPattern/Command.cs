using System;
using System.Diagnostics;
using ESRI.ArcGIS.SOESupport;

namespace CommandPattern {

    public abstract class Command<T> : Command where T : class {
        public T Result { get; protected set; }
    }

    public abstract class Command {
        /// <summary>
        ///     The message code to be used for all failed commands
        /// </summary>
        public const int MessageCode = 2472;

#if !DEBUG
        public ServerLogger Logger = new ServerLogger();
#endif
        public abstract void Execute();
        public abstract override string ToString();

        public virtual void Run()
        {
            var commandName = ToString();

            try
            {
                Debug.Print("Executing\r\n{0}", commandName);
#if !DEBUG

                Logger.LogMessage(ServerLogger.msgType.infoSimple, string.Format("{0}.{1}", commandName, "execute"),
                                  MessageCode,
                                  string.Format("Executing\r\n{0}", commandName));
#endif

                Execute();
                Debug.Print("Done Executing\r\n{0}", commandName);
#if !DEBUG

                Logger.LogMessage(ServerLogger.msgType.infoSimple, string.Format("{0}.{1}", commandName, "execute"),
                                  MessageCode,
                                  "Done Executing");
#endif
            }
            catch (Exception ex)
            {
                Debug.Print("Error processing task: {0} {1}", commandName, ex);

#if !DEBUG
                Logger.LogMessage(ServerLogger.msgType.error, string.Format("{0}.{1}", commandName, "execute"),
                                  MessageCode,
                                  "Error running command");
#endif
                throw ex;
            }
            finally
            {
#if !DEBUG
                Logger = null;
#endif
            }
        }
    }

}