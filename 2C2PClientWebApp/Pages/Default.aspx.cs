using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace _2C2PClientWebApp.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        TCTPService.TCTPServiceClient tc2pService = new TCTPService.TCTPServiceClient();
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        private string GetIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            string IPAddress = (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).FirstOrDefault();

            return IPAddress;
        }
       
        protected void btnChooseFile_Click(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string AuthCode = "2c2p^&*";
                if (openFileChooser.HasFile && (openFileChooser.PostedFile != null) && (openFileChooser.PostedFile.ContentLength > 0))
                {
                    string fe = System.IO.Path.GetExtension(openFileChooser.FileName).ToLower();
                    fe = fe.Remove(0, 1);
                    if(openFileChooser.PostedFile.ContentLength>1048576)
                    {
                        Response.Write("<script LANGUAGE='JavaScript' >alert('Failed, your selected file size is larger than expected!')</script>");
                        return;
                    }
                    if (fe == "csv" || fe == "xml")
                    {
                        FileUploadStatus.Text = "File Selected.";
                        string fn = DateTime.Now.ToString("HHmmss")+System.IO.Path.GetFileName(openFileChooser.PostedFile.FileName);
                        string FileLocation = Server.MapPath("upload") + "\\" + fn;
                        try
                        {
                            openFileChooser.PostedFile.SaveAs(FileLocation);
                            string requestBody = "";
                            requestBody = File.ReadAllText(FileLocation);
                            string IPAddress = GetIPAddress();
                            string response = tc2pService.InsertTransaction(AuthCode, Dns.GetHostName(), IPAddress, fe, requestBody);

                            if (response == "200 OK")
                            {
                                Response.Write("<script LANGUAGE='JavaScript' >alert('Post Succeed.')</script>");
                            }
                            else
                            {
                                Response.Write("<script LANGUAGE='JavaScript' >alert('Fail!')</script>");
                            }
                        }
                        catch (Exception ex)
                        {
                            FileUploadStatus.Text = "Error: " + ex.Message;
                        }
                    }
                    else
                    {
                        FileUploadStatus.Text = "Unknown format.";
                    }

                }
                else
                {
                    FileUploadStatus.Text = "Please select a file to upload.";
                }
            }
        }
        protected void FromDateChange(object sender, EventArgs e)
        {
            txtDateFrom.Text = fromDateCal.SelectedDate.ToShortDateString();
        }
        protected void ToDateChange(object sender, EventArgs e)
        {
            txtDateTo.Text = toDateCal.SelectedDate.ToShortDateString();
        }
        protected void btnExecuteQuery_Click(object sender, EventArgs e)
        {
            try {
                if (IsPostBack)
                {
                    txtResult.Text = "";
                    string AuthCode = "Employ Me";
                    string IPAddress = GetIPAddress();
                    string DateFrom = "", DateTo = "", currency = "", status = "";
                    if (!string.IsNullOrEmpty(txtDateFrom.Text) && !string.IsNullOrEmpty(txtDateTo.Text))
                    {
                        DateFrom = txtDateFrom.Text;
                        DateTo = txtDateTo.Text;
                        if (DateTime.Parse(DateFrom) > DateTime.Parse(DateTo))
                        {
                            Response.Write("<script LANGUAGE='JavaScript' >alert('Invalid, FromDate is later than ToDate.')</script>");
                            return;
                        }

                    }
                    if (dropListCurrency.SelectedValue != "Select")
                    {
                        currency = dropListCurrency.SelectedValue;
                    }
                    if (dropListStatus.SelectedValue != "Select")
                    {
                        status = dropListStatus.SelectedValue;
                    }
                    string response = tc2pService.GetTransactions(AuthCode, currency, DateFrom, DateTo, status);
                    txtResult.Text = response;

                }
            }catch(Exception ex)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('"+ex.ToString()+"')</script>");

            }
        }
        
    }
}