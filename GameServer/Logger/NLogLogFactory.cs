using NLog;

namespace GameServer;

#if (__NOT_USE_NLOG__ != true)  //NLog를 사용하지 않는다면 __NOT_USE_NLOG__ 선언한다
    public class NLogLogFactory : SuperSocket.SocketBase.Logging.LogFactoryBase
    {
        public NLogLogFactory()
            : this("NLog.config")
        {
        }

        public NLogLogFactory(string nlogConfig)
            : base(nlogConfig)
        {
            if (!IsSharedConfig)
            {
                LogManager.Setup().LoadConfigurationFromFile(new[] { ConfigFile });
            }
            else
            {                
            }
        }

        public override SuperSocket.SocketBase.Logging.ILog GetLog(string name)
        {
            return new NLogLog(NLog.LogManager.GetLogger(name));
        }
    }
#endif