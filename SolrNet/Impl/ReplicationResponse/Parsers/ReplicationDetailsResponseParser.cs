#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Globalization;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl.ResponseParsers 
{
    /// <summary>
    /// Parses header (status, QTime, etc) and details from a response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class ReplicationDetailsResponseParser<T> : ReplicationAbstractResponseParser<T>, ISolrAbstractResponseParser<T>, ISolrReplicationDetailsResponseParser
    {
        /// <summary>
        /// Parses XML response to response class
        /// </summary>
        /// <param name="response">XML</param>
        /// <returns>ReplicationDetailsResponse class</returns>
        public ReplicationDetailsResponse Parse(XDocument response)
        {            
            var responseHeader = ParseHeader(response);
            if (responseHeader == null)
            {
                return null;
            }

            var responseIndexSizeNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='indexSize']");
            var indexSize = responseIndexSizeNode != null ? responseIndexSizeNode.Value : string.Empty;

            var responseIndexPathNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='indexPath']");
            var indexPath = responseIndexPathNode != null ? responseIndexPathNode.Value : string.Empty;

            var responseIsMasterNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='isMaster']");
            var isMaster = responseIsMasterNode != null ? responseIsMasterNode.Value : string.Empty;

            var responseIsSlaveNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='isSlave']");
            var isSlave = responseIsSlaveNode != null ? responseIsSlaveNode.Value : string.Empty;

            long indexVersion = -1;
            var responseIndexVersionNode = response.XPathSelectElement("response/lst[@name='details']/long[@name='indexVersion']");
            if (responseIndexVersionNode != null)
            {
                indexVersion = long.Parse(responseIndexVersionNode.Value, CultureInfo.InvariantCulture.NumberFormat);
            }

            long generation = -1;
            var responseGenerationNode = response.XPathSelectElement("response/lst[@name='details']/long[@name='generation']");
            if (responseGenerationNode != null)
            {
                generation = long.Parse(responseGenerationNode.Value, CultureInfo.InvariantCulture.NumberFormat);
            }

            //slave node
            string isReplicating = null;
            string totalPercent = null;
            string timeRemaining = null;
            var responseSlaveNode = response.XPathSelectElement("response/lst[@name='details']/lst[@name='slave']");
            if (responseSlaveNode != null)
            {
                var isReplicatingTemp = responseSlaveNode.XPathSelectElement("str[@name='isReplicating']");
                isReplicating = isReplicatingTemp != null ? isReplicatingTemp.Value : null;

                if (isReplicating != null && isReplicating.ToLower() == "true")
                {
                    var totalPercentTemp = responseSlaveNode.XPathSelectElement("str[@name='totalPercent']");
                    totalPercent = totalPercentTemp != null ? totalPercentTemp.Value : null;

                    var timeRemainingTemp = responseSlaveNode.XPathSelectElement("str[@name='timeRemaining']");
                    timeRemaining = timeRemainingTemp != null ? timeRemainingTemp.Value : null;
                }
            }

            return new ReplicationDetailsResponse(responseHeader, indexSize, indexPath, isMaster, isSlave, indexVersion, generation, isReplicating, totalPercent, timeRemaining);
        }
    }
}
