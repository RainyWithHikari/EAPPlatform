using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace EAP.Dashboard.Utils
{
    public class EmailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="MessageSubject">邮件主题</param>
        /// <param name="MessageBody">邮件正文</param>
        /// <returns></returns>
        public static bool SendMail(string[] MessageTo, string MessageSubject, string MessageBody, List<Attachment> attachments = null)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);//此处注册只为防止个别邮箱证书验证失败问题
            MailAddress MessageFrom = new MailAddress("zj.sys.smd_systems@usiglobal.com");
            MailMessage message = new MailMessage();
            message.From = MessageFrom;
            foreach (var mt in MessageTo)
            {
                message.To.Add(mt);             //收件人邮箱地址可以是多个以实现群发
            }
            message.Subject = MessageSubject;
            message.Body = MessageBody;
            message.IsBodyHtml = true;              //是否为html格式
            message.Priority = MailPriority.High;  //发送邮件的优先等级
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }

            SmtpClient sc = new SmtpClient();
            sc.Host = "hybrid-cn.usiglobal.com";              //指定发送邮件的服务器地址或IP
            sc.Port = 25;//指定发送邮件端口
            sc.UseDefaultCredentials = true;
            sc.EnableSsl = false;
            sc.Credentials = new NetworkCredential("zj.sys.smd_systems@usiglobal.com", "Usish@2022");//指定登录服务器的用户名和密码
            try
            {
                sc.Send(message);      //发送邮件
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}