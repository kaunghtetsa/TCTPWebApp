using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

public class CodeLogic
    {
    
    }
public class TransactionReceiver
   {
    public string TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; }
    public string Status { get; set; }
}
public class TransactionResult
{
    public string id { get; set; }
    public string payment { get; set; }
    public string Status { get; set; }
}