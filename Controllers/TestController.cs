using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Data.SqlClient;
using Hangfire;

namespace Sozlesme_Takip_Sistemi1.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> SendEmail()
        {
            var expiringContracts = GetExpiringContracts();

            foreach (var contract in expiringContracts)
            {
                
               var  message = $" {contract.ContractId} numaralı sözleşmeniz {contract.EndDate.ToString("dd.MM.yyyy")} tarihinde sona erecektir.";

                await SendEmailAsync(contract.Email, "Contract Expiration Notice", message);
            }

            return Json(0, JsonRequestBehavior.AllowGet);
        }

        

        public async static Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var _email = "tt5559300@gmail.com";
                var _epass = ConfigurationManager.AppSettings["EmailPassword"];
                var _dispName = "Test Mail";
                MailMessage myMessage = new MailMessage();
                myMessage.To.Add(email);
                myMessage.From = new MailAddress(_email, _dispName);
                myMessage.Subject = subject;
                myMessage.Body = message;
                myMessage.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.EnableSsl = true;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_email, _epass);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.SendCompleted += (s, e) => { smtp.Dispose(); };
                    await smtp.SendMailAsync(myMessage);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<ExpiringContract> GetExpiringContracts()
        {
            List<ExpiringContract> contracts = new List<ExpiringContract>();

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string query = @"
                SELECT c.id AS contract_id, c.end_date, u.email
                FROM contracts c
                JOIN users u ON c.department_id = u.department_id
                WHERE c.end_date <= DATEADD(day, 30, GETDATE())";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    contracts.Add(new ExpiringContract
                    {
                        ContractId = reader.GetInt32(0),
                        EndDate = reader.GetDateTime(1),
                        Email = reader.GetString(2)
                    });
                }
            }

            return contracts;
        }

        private class ExpiringContract
        {
            public int ContractId { get; set; }
            public DateTime EndDate { get; set; }
            public string Email { get; set; }
        }
    }
    public class MyJobActivator : JobActivator
    {
        public override object ActivateJob(Type jobType)
        {
            // Hangfire'ın bu türü yönetmesine izin vermek için null döndür
            return null;
        }
    }


}
