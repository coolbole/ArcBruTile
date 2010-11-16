using System.Net;

namespace BruTileArcGIS
{
    /// <summary>
    /// Class containing the state object for GeoRSS requests
    /// </summary>
    internal class KMLWebRequestStateObject
    {
        private WebRequest webRequest;

        /// <summary>
        /// Web request
        /// </summary>
        public WebRequest WebRequest
        {
            get { return webRequest; }
            set { webRequest = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webRequest"></param>
        public KMLWebRequestStateObject(WebRequest webRequest)
        {
            this.webRequest = webRequest;
        }
    }
}
