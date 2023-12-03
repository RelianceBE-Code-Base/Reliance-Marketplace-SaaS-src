using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Mail;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Extensions.Configuration;
using sib_api_v3_sdk.Model;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Api;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Service to send emails using SMTP settings.
/// </summary>
/// <seealso cref="IEmailService" />
public class SMTPEmailService : IEmailService
{
    /// <summary>
    /// The application configuration repository.
    /// </summary>
    private readonly IApplicationConfigRepository applicationConfigRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SMTPEmailService"/> class.
    /// </summary>
    /// <param name="applicationConfigRepository">The application configuration repository.</param>
    public SMTPEmailService(IApplicationConfigRepository applicationConfigRepository)
    {
        this.applicationConfigRepository = applicationConfigRepository;
    }

    /// <summary>
    /// Sends the email.
    /// </summary>
    /// <param name="emailContent">Content of the email.</param>
    public void SendEmail(EmailContentModel emailContent)
    {
        if (!string.IsNullOrEmpty(emailContent.ToEmails) || !string.IsNullOrEmpty(emailContent.BCCEmails))
        {
            //mail.From = new MailAddress(emailContent.FromEmail);
            //mail.IsBodyHtml = true;
            //mail.Subject = emailContent.Subject;
            //mail.Body = emailContent.Body;

            //string[] toEmails = emailContent.ToEmails.Split(';');
            //foreach (string multimailid in toEmails)
            //{
            //    mail.To.Add(new MailAddress(multimailid));
            //}

            //if (!string.IsNullOrEmpty(emailContent.BCCEmails))
            //{
            //    foreach (string multimailid1 in toEmails)
            //    {
            //        mail.Bcc.Add(new MailAddress(multimailid1));
            //    }
            //}

            //SmtpClient smtp = new SmtpClient();
            //smtp.Host = emailContent.SMTPHost;
            //smtp.Port = emailContent.Port;
            //smtp.UseDefaultCredentials = false;
            //smtp.Credentials = new NetworkCredential(
            //    emailContent.UserName, emailContent.Password);
            //smtp.EnableSsl = emailContent.SSL;
            //smtp.Send(mail);

            Configuration.Default.ApiKey["api-key"] = this.applicationConfigRepository.GetValueByName("SMTPPassword");
            var apiInstance = new TransactionalEmailsApi();

            string SenderName = this.applicationConfigRepository.GetValueByName("SMTPUserName");
            string SenderEmail = this.applicationConfigRepository.GetValueByName("SMTPFromEmail");
            SendSmtpEmailSender emailSender = new SendSmtpEmailSender(SenderName, SenderEmail);

            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            string[] toEmails = emailContent.ToEmails.Split(';');
            foreach (string multimailid in toEmails)
            {
                SendSmtpEmailTo emailReceiver = new SendSmtpEmailTo(multimailid, null);
                To.Add(emailReceiver);
            }

            string HtmlContent = emailContent.Body;
            string TextContent = null;
            string Subject = emailContent.Subject;


            try
            {
                var sendSmtpEmail = new SendSmtpEmail(emailSender, To, null, null, HtmlContent, TextContent, Subject);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}