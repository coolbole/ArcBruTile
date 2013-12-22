using System;
using System.Threading;
using log4net;

namespace BruTileArcGIS
{
    public class TileLoader
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger("ArcBruTileSystemLogger");
        private BruTileHelper brutileHelper;

        //threading
        private ManualResetEvent _doneEvent;

        public TileLoader(BruTileHelper pbrutileHelper, ManualResetEvent doneEvent)
        {
            brutileHelper = pbrutileHelper;
            _doneEvent = doneEvent;
        }

        // Wrapper method for use with thread pool.
        //params uit threadContext zijn i=threadnr, RequestBuilder, TileInfo
        public void ThreadPoolCallback(object threadContext)
        {
            try
            {
                object[] parameters = (object[])threadContext;
                int threadIndex = (int)parameters[0];
                logger.Debug(string.Format("thread {0} started...", threadIndex));
                //bool result = brutileHelper.GetTileOnThreadPool(threadContext);
                logger.Debug(string.Format("thread {0} result calculated...", threadIndex));
                _doneEvent.Set();
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Fout in ThreadPoolCallback {0} ", ex.Message));
            }
        }
    }
}