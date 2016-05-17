using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl.ResponseParsers
{
    /// <summary>
    /// Base parser for replication responses
    /// </summary>
    public abstract class ReplicationAbstractResponseParser<T>
    {
        /// <summary>
        /// Header parser
        /// </summary>
        /// <param name="xml">XML</param>
        /// <param name="results">results</param>
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results)
        {
            var header = ParseHeader(xml);
            if (header != null)
            {
                results.Header = header;
            }
        }

        /// <summary>
        /// Parses response header out of xdocument
        /// </summary>
        /// <param name="response">xml document</param>
        /// <returns>ResponseHeader</returns>
        protected ResponseHeader ParseHeader(XDocument response)
        {
            var responseHeaderNode = response.XPathSelectElement("response/lst[@name='responseHeader']");
            if (responseHeaderNode == null)
            {
                return null;
            }
            var responseHeader = ParseHeader(responseHeaderNode);
            return responseHeader;
        }

        /// <summary>
        /// Parses response header
        /// </summary>
        /// <param name="node">XML</param>
        /// <returns>ResponseHeader</returns>
        public ResponseHeader ParseHeader(XElement node)
        {
            var responseHeader = new ResponseHeader();
            responseHeader.Status = int.Parse(node.XPathSelectElement("int[@name='status']").Value, CultureInfo.InvariantCulture.NumberFormat);
            responseHeader.QTime = int.Parse(node.XPathSelectElement("int[@name='QTime']").Value, CultureInfo.InvariantCulture.NumberFormat);
            responseHeader.Params = new Dictionary<string, string>();

            var paramNodes = node.XPathSelectElements("lst[@name='params']/str");
            foreach (var n in paramNodes)
            {
                responseHeader.Params[n.Attribute("name").Value] = n.Value;
            }

            return responseHeader;
        }
    }
}
