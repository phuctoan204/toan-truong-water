using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;
using System;

public interface IEmailService
{
    Task SendReminderEmailAsync(string toEmail);
    void SendMail(string toEmail);
}

public class SendEmailService : IEmailService
{
    public async Task SendReminderEmailAsync(string toEmail)
    {
        var smtpClient = new SmtpClient("https://localhost:44375/")
        {
            Port = 587,
            Credentials = new NetworkCredential("phuctoan351@gmail.com", "125cangiuoc"),
            EnableSsl = true,
        };

        var mail = new MailMessage("phuctoan3351@gmail.com", toEmail)
        {
            Subject = "Bạn quên thanh toán giỏ hàng!",
            Body = "Bạn đã thêm sản phẩm vào giỏ hàng nhưng chưa thanh toán. Hãy quay lại và hoàn tất đơn hàng của bạn nhé!",
            IsBodyHtml = false,
        };

        await smtpClient.SendMailAsync(mail);
    }

    public void SendMail(string toMail)
    {
        string senderEmail = "phuctoan351@gmail.com";
        string senderPassword = "ajyhkpjpcjedydza";

        // Recipient's email address
        string recipientEmail = toMail;

        // Mail message
        MailMessage mail = new MailMessage(senderEmail, recipientEmail);
        //đổi lại tiêu đề mail
        mail.Subject = "PhucToan Water";
        //đôi lại nội dung mail
        mail.Body = "Bạn đã thêm sản phẩm vào giỏ hàng nhưng chưa thanh toán. Hãy quay lại và hoàn tất đơn hàng của bạn nhé!.......";

        // SMTP client configuration
        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"); // Replace with your SMTP server address
        smtpClient.Port = 587; // Typically, port 587 is used for SMTP with TLS
        smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
        smtpClient.EnableSsl = true; // Enable SSL/TLS

        try
        {
            // Send the email
            smtpClient.Send(mail);
            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email: " + ex.Message);
        }
    }
}
