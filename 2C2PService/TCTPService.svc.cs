using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace _2C2PService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TCTPService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TCTPService.svc or TCTPService.svc.cs at the Solution Explorer and start debugging.
    public class TCTPService : ITCTPService
    {
        ServiceRepository sr = new ServiceRepository();
        public string InsertTransaction(string AuthenticationKey, string UserAgentName, string IPAddress, string FileType, string RequestBody)
        {
            int ServiceAccessRightID = sr.AuthenticateUser(AuthenticationKey, "CREATE");
            if (ServiceAccessRightID > 0)
            {
                int CallLogID = sr.CreateCallLog(UserAgentName, IPAddress, FileType, ServiceAccessRightID);
                if (CallLogID > 0)
                {
                    TCTPEntities tpEntity = new TCTPEntities();

                    var iCallLogvar = tpEntity.CallLogs.Find(CallLogID);

                    bool IsXML = false;
                    if (FileType == "xml")
                    {
                        IsXML = true;
                    }
                    int iTransCount = sr.InsertTransactions(RequestBody, IsXML, CallLogID);
                    if (iTransCount > 0)
                    {
                        iCallLogvar.IsSucceed = true;
                        iCallLogvar.TransactionCount = iTransCount;
                        tpEntity.SaveChanges();
                        return "200 OK";
                    }
                    else
                    {
                        iCallLogvar.IsSucceed = false;
                        tpEntity.SaveChanges();
                        return "597 Internal services error(Error in InsertTransactions)";
                    }
                }
                return "598 Internal services error(Can not create Call Log)";
            }
            return "599 Internal services error(AuthenticationKey is not valid)";
        }

        public string GetTransactions(string AuthenticationKey, string ByCurrency, string DateTimeFrom, string DateTimeTo, string ByStatus)
        {
            int ServiceAccessRightID = sr.AuthenticateUser(AuthenticationKey, "GET");
            if (ServiceAccessRightID > 0)
            {
                List<TransactionResult> transRec = sr.GetTransactions(ByCurrency, DateTimeFrom, DateTimeTo, ByStatus);
                if (transRec != null)
                {
                    List<GetJSONTransactions> jsonRes = ResponseFormator(transRec);
                    return JsonConvert.SerializeObject(jsonRes);
                }
                return null;
            }
            return null;
        }
        //[DataContract]
        //public class JSONResult
        //{
        //    [DataMember(Name = "Result", Order = 0)]
        //    public GetJSONTransactions res { get; set; }

        //}
        [DataContract]
        public class GetJSONTransactions
        {
            [DataMember(Name = "id", Order = 0)]
            public string id { get; set; }
            [DataMember(Name = "payment", Order = 1)]
            public string payment { get; set; }
            [DataMember(Name = "Status", Order = 2)]
            public string Status { get; set; }
        }
        private List<GetJSONTransactions> ResponseFormator(List<TransactionResult> res)
        {
            List<GetJSONTransactions> iRes = new List<GetJSONTransactions>();
            for (int i = 0; i < res.Count; i++)
            {
                GetJSONTransactions jresult = new GetJSONTransactions
                {
                    id = res[i].id,
                    payment = res[i].payment,
                    Status = res[i].Status
                };
                iRes.Add(jresult);
            }

            return iRes;
        }

    }
}
