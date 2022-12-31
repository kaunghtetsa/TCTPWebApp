using _2C2PService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml;

public class ServiceRepository
    {
    TCTPEntities tc2pEntities = new TCTPEntities();
    #region crud methods
    /// <summary>
    /// AddCallLog
    /// </summary>
    /// <param name="UserAgentName"></param>
    /// <param name="IPAddress"></param>
    /// <param name="FileName"></param>
    /// <param name="FileType"></param>
    /// <param name="FileSize_Byte"></param>
    /// <param name="TransactionCount"></param>
    /// <param name="IsSucceed"></param>
    /// <param name="ServiceAccessRightID"></param>
    /// <returns></returns>
    public int CreateCallLog(string UserAgentName,string IPAddress, string FileType, int ServiceAccessRightID)
    {
        try
        {
            CallLog iNewLog =VerifyCallLog(UserAgentName, IPAddress, FileType, ServiceAccessRightID);
            if(iNewLog!=null)
            {
                tc2pEntities.CallLogs.Add(iNewLog);
                tc2pEntities.Entry(iNewLog).State = System.Data.Entity.EntityState.Added;
                tc2pEntities.SaveChanges();
                return iNewLog.ID;
            }
            return 0;
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex.ToString(), "CreateCallLog");
            return 0;
        }
    }


    #region insert data
    public int InsertTransactions(string RequestBody,bool IsXML,int CallLogID)
    {
        try
        {
            List<TransactionReceiver> transRec = new List<TransactionReceiver>();
            if (IsXML)
            {
                DataSet dsXML = DeserializeXmlStringToObject(RequestBody);
                transRec = DataSetToTransactionList(dsXML);
            }
            else
            {
                transRec = DeserializeCSVStringToObject(RequestBody);
            }
            if (transRec == null || transRec.Count==0) return 0;
             
            if (VerifyTransactionRecord(transRec))
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (var context = new TCTPEntities())
                    {

                        for (int i = 0; i < transRec.Count; i++)
                        {
                            if(transRec[i].Status=="Approved")
                            {
                                transRec[i].Status = "A";
                            }
                            else if (transRec[i].Status == "Failed" || transRec[i].Status == "Rejected")
                            {
                                transRec[i].Status = "R";
                            }
                            else if (transRec[i].Status == "Finished" || transRec[i].Status == "Done")
                            {
                                transRec[i].Status = "D";
                            }
                            TransactionRecord iNewTransaction = new TransactionRecord
                            {
                                TransactionID = transRec[i].TransactionId,
                                CallLogID = CallLogID,
                                Amount = transRec[i].Amount,
                                CurrencyCode = transRec[i].CurrencyCode.Trim(),
                                TransactionDateTime = transRec[i].TransactionDate,
                                Status = transRec[i].Status,
                                CreatedOn = DateTime.Now,
                                Active = true
                            };
                            context.TransactionRecords.Add(iNewTransaction);
                            context.SaveChanges();

                        }
                    }
                    scope.Complete();
                }
                return transRec.Count;
            }
            return 0;
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex.ToString(), "InsertTransaction");
            return 0;
        }
    }

    #endregion

    #region get data
    public List<TransactionResult> GetTransactions(string ByCurrency, string DateTimeFrom, string DateTimeTo, string ByStatus)
    {
        List<string> statusList = new List<string>{"A","R","D"};
        IQueryable<TransactionResult> tranResult;
        if (StringChecker(DateTimeFrom) && StringChecker(DateTimeFrom))
        {
            DateTime dtFrom,dtTo;
            DateTime.TryParse(DateTimeFrom,out dtFrom);

            DateTime.TryParse(DateTimeTo, out dtTo);
            if (dtFrom <= dtTo)
            {
                tranResult = tc2pEntities.TransactionRecords.Where(x => x.TransactionDateTime >= dtFrom && x.TransactionDateTime <= dtTo
                && x.Active == true).Select(S => new TransactionResult()
                {
                    id = S.TransactionID,
                    payment = S.Amount + " " + S.CurrencyCode.Trim(),
                    Status = S.Status
                });
            }
            else return null;
        }
        else
        {
            tranResult = tc2pEntities.TransactionRecords.Where(x => x.Active == true).Select(S => new TransactionResult()
                {
                    id = S.TransactionID,
                    payment = S.Amount + " " + S.CurrencyCode,
                    Status = S.Status
                });
        }
        if(StringChecker(ByCurrency) && ByCurrency.Length==3)
        {
            tranResult=tranResult.Where(x => x.payment.EndsWith(" " + ByCurrency));
        }
        if (StringChecker(ByStatus) && statusList.Contains(ByStatus))
        {
            tranResult=tranResult.Where(x => x.Status==ByStatus);
        }
        if(tranResult!=null)
        {
            return tranResult.ToList();
        }
        else
        {
            return null;
        }
        
    }
    #endregion
    #endregion
    #region local methods

    private List<TransactionReceiver> DataSetToTransactionList(DataSet dsXML)
    {
        try
        {
            List<TransactionReceiver> tReceiver = new List<TransactionReceiver>();
            tReceiver.Clear();
            if (dsXML != null && dsXML.Tables.Count > 0 && dsXML.Tables.Contains("Transaction") && dsXML.Tables.Contains("PaymentDetails"))
            {
                for (int i = 0; i < dsXML.Tables["Transaction"].Rows.Count; i++)
                {
                    TransactionReceiver tRow = new TransactionReceiver
                    {
                        TransactionId = dsXML.Tables["Transaction"].Rows[i]["id"].ToString(),
                        TransactionDate = DateTime.Parse(dsXML.Tables["Transaction"].Rows[i]["TransactionDate"].ToString()),
                        Status = dsXML.Tables["Transaction"].Rows[i]["Status"].ToString(),
                        Amount = Decimal.Parse(dsXML.Tables["PaymentDetails"].Rows[i]["Amount"].ToString()),
                        CurrencyCode = dsXML.Tables["PaymentDetails"].Rows[i]["CurrencyCode"].ToString()
                    };
                    tReceiver.Add(tRow);
                }

            }
            return tReceiver;
        }
        catch (Exception ex)
        {
            WriteErrorLog(ex.ToString(), "DataSetToTransactionList");
            return null;
        }
    }

    private string SHA256Hasing(string OrgValue)
    {
        string hashedValue = string.Empty;
        SHA256CryptoServiceProvider hashAlgorithm = new SHA256CryptoServiceProvider();

        if (!string.IsNullOrEmpty(OrgValue))
        {
            byte[] hashedData = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(OrgValue));

            foreach (byte b in hashedData)
            {
                hashedValue += String.Format("{0,2:x2}", b);
            }
        }
        return hashedValue;
    }
    public int AuthenticateUser(string AuthenticationKey, string PermissionLevel)
    {
        AuthenticationKey = SHA256Hasing(AuthenticationKey + "123!@#");
        var iPermission = tc2pEntities.ServiceAccessRights.Where(x => x.AuthenticationKey == AuthenticationKey && x.PermissionLevel == PermissionLevel.ToUpper() && x.Active==true);
        if (iPermission != null)
        {
            return iPermission.FirstOrDefault().ID;
        }
        return 0;
    }
    private bool StringChecker(string iPara)
    {
       if(string.IsNullOrEmpty(iPara))
        {
            return false;
        }
        else if (string.IsNullOrWhiteSpace(iPara.Trim()))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Verify Call Log
    /// </summary>
    /// <param name="UserAgentName"></param>
    /// <param name="IPAddress"></param>
    /// <param name="FileName"></param>
    /// <param name="FileType"></param>
    /// <param name="FileSize_Byte"></param>
    /// <param name="TransactionCount"></param>
    /// <param name="IsSucceed"></param>
    /// <param name="ServiceAccessRightID"></param>
    /// <returns></returns>
    private CallLog VerifyCallLog(string UserAgentName, string IPAddress, string FileType, int ServiceAccessRightID)
    {
        try
        {
            CallLog iNewLog = new CallLog();
            bool bAuthenticator = true;
            //Authenticate UserAgentName
            if (StringChecker(UserAgentName))
            {
                iNewLog.UserAgentName = UserAgentName;
            }
            else
            {
                iNewLog.UserAgentName = "Invalid Data";
                bAuthenticator = false;
            }

            //Authenticate IPAddress
            if (StringChecker(IPAddress))
            {
                iNewLog.IPAddress = IPAddress;
            }
            else
            {
                iNewLog.IPAddress = "Invalid Data";
                bAuthenticator = false;
            }

            //Authenticate FileType
            if (StringChecker(FileType))
            {
                iNewLog.FileType = FileType;
            }
            else
            {
                iNewLog.FileType = "Invalid Data";
                bAuthenticator = false;
            }

            //Authenticate ServiceAccessRightID
            if (ServiceAccessRightID > 0)
            {
                iNewLog.ServiceAccessRightID = ServiceAccessRightID;
            }
            else
            {
                iNewLog.ServiceAccessRightID = 0;
                bAuthenticator = false;
            }

            //Authenticate IsSucceed
            if (bAuthenticator)
            {
                iNewLog.IsSucceed = true;
            }
            else
            {
                iNewLog.IsSucceed = false;
            }
            iNewLog.PushedOn = DateTime.Now;
            iNewLog.Active = true;
            return iNewLog;
        }
        catch(Exception ex)
        {
            WriteErrorLog(ex.ToString(), "VerifyCallLog");
            return null;
        }
    }


    private bool VerifyTransactionRecord(List<TransactionReceiver> TransRec)
    {
        for (int i = 0; i < TransRec.Count; i++)
        {
            if (!StringChecker(TransRec[i].TransactionId))
            {
               return false;
            }

            if (!StringChecker(TransRec[i].CurrencyCode) || TransRec[i].CurrencyCode.Length!=3)
            {
                return false;
            }           
        }
        return true;
    }
    private static DataSet DeserializeXmlStringToObject(string xmlString)
    {
        

        var xmldoc = new XmlDocument();
        xmldoc.LoadXml(xmlString);
        var fromXml = JsonConvert.SerializeXmlNode(xmldoc);
        string jsonString = "{ \"rootNode\": {" + fromXml.Trim().TrimStart('{').TrimEnd('}') + @"} }";
        var xd = JsonConvert.DeserializeXmlNode(jsonString);

        //// DataSet is able to read from XML and return a proper DataSet
        var result = new DataSet();
        result.ReadXml(new XmlNodeReader(xd), XmlReadMode.Auto);
        return result;
    }

    private List<TransactionReceiver> DeserializeCSVStringToObject(string RequestBody)
    {
        try
        {
            List<TransactionReceiver> transRec = new List<TransactionReceiver>();
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<string> currencyList = new List<string> { "Approved", "Failed", "Finished" };
            foreach (string row in RequestBody.Split('\n'))
            {

                if (!string.IsNullOrEmpty(row) && row.Trim().Length > 1)
                {
                    //Execute a loop over the columns.  
                    string[] wholerowcell = row.Split(',');
                    if (wholerowcell.Length == 5)
                    {
                        var trecRow = new TransactionReceiver
                        {
                            TransactionId = wholerowcell[0],
                            Amount = decimal.Parse(wholerowcell[1]),
                            CurrencyCode = wholerowcell[2],
                            TransactionDate = DateTime.ParseExact(wholerowcell[3], "dd/MM/yyyy HH:mm:ss", provider),
                            Status = wholerowcell[4].Replace(',', ' ').Trim()
                        };
                        transRec.Add(trecRow);
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            return transRec;
        }
        catch(Exception ex)
        {
            WriteErrorLog(ex.ToString(), "DeserializeCSVStringToObject");
            return null;
        }
    }
    public void WriteErrorLog(string eMessage, string ErrorModule)
    {
        try
        {
            if (!string.IsNullOrEmpty(eMessage))
            {
                string AppPath = System.Configuration.ConfigurationManager.AppSettings.Get("AppPath");
                string TempYear = DateTime.Now.Year.ToString();
                string Tempmonth = DateTime.Now.Month.ToString();
                string TempDay = DateTime.Now.Day.ToString();

                if (Tempmonth.Length == 1) Tempmonth = "0" + Tempmonth;
                if (TempDay.Length == 1) TempDay = "0" + TempDay;

                System.IO.StreamWriter y; string revdate = DateTime.Now.ToLongDateString();
                y = System.IO.File.AppendText(AppPath + "\\LogFile\\E" + TempYear + Tempmonth + TempDay + ".log");
                y.WriteLine("[" + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToString("HH",
                    CultureInfo.InvariantCulture) + ":" + DateTime.Now.ToString("mm", CultureInfo.InvariantCulture) +
                    ":" + DateTime.Now.ToString("ss", CultureInfo.InvariantCulture) + "] " + "[" + ErrorModule + "]" + " Message: " + eMessage);
                y.Close();
            }
        }
        catch
        {
           
        }
    }
    #endregion
}
