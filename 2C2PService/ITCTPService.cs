using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using static _2C2PService.TCTPService;

namespace _2C2PService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITCTPService" in both code and config file together.
    [ServiceContract]
    public interface ITCTPService
    {
        //[OperationContract]
        //[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        //I think time is not enough to cover implementation of this api , so I choose to develop a webservices
        [OperationContract]
        string InsertTransaction(string AuthenticationKey, string UserAgentName, string IPAddress, string FileType, string RequestBody);

        [OperationContract]
        string GetTransactions(string AuthenticationKey, string ByCurrency, string DateTimeFrom, string DateTimeTo, string ByStatus);


    }
}
