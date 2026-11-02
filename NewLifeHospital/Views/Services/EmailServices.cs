using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NewLifeHospital.Models;

namespace NewLifeHospital.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;


    public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Contact Form Email
        public async Task SendContactEmailAsync(ContactFormModel model)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var receiverEmail = _configuration["EmailSettings:ReceiverEmail"];
            var appPassword = _configuration["EmailSettings:AppPassword"];

            if (string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(receiverEmail) ||
                string.IsNullOrWhiteSpace(appPassword))
            {
                throw new InvalidOperationException(
                    "Email settings are missing. Check your configuration.");
            }

            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(senderEmail));
            email.To.Add(MailboxAddress.Parse(receiverEmail));
            email.ReplyTo.Add(MailboxAddress.Parse(model.Email));

            email.Subject = string.IsNullOrWhiteSpace(model.Subject)
                ? "New Contact Message - NewLifeHospital"
                : $"Contact: {model.Subject}";

            email.Body = new TextPart("plain")
            {
                Text =
                    $"New contact message received from NewLifeHospital website.\n\n" +
                    $"Name: {model.Name}\n" +
                    $"Email: {model.Email}\n" +
                    $"Phone: {model.Phone ?? "Not provided"}\n" +
                    $"Subject: {model.Subject ?? "Not provided"}\n\n" +
                    $"Message:\n{model.Message}"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(senderEmail, appPassword);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        // Appointment Approval / Rejection Email
        public async Task SendAppointmentStatusEmailAsync(
            Appointment appointment)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var appPassword = _configuration["EmailSettings:AppPassword"];

            if (string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(appPassword))
            {
                throw new InvalidOperationException(
                    "Email settings are missing.");
            }

            if (string.IsNullOrWhiteSpace(appointment.Email))
            {
                throw new InvalidOperationException(
                    "Patient email address is missing.");
            }

            var isApproved = appointment.Status == "Approved";

            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(senderEmail));
            email.To.Add(MailboxAddress.Parse(appointment.Email));

            email.Subject = isApproved
                ? "Appointment Approved - NewLife Hospital"
                : "Appointment Rejected - NewLife Hospital";

            email.Body = new TextPart("plain")
            {
                Text = isApproved

                    ? $@"Dear {appointment.PatientName},


Good news! Your appointment at NewLife Hospital has been approved.

Appointment Details:

Patient Name: {appointment.PatientName}
Doctor: {appointment.Doctor}
Date: {appointment.AppointmentDate:dd MMMM yyyy}
Time: {appointment.AppointmentTime}
Status: Approved

Thank you for choosing NewLife Hospital.

Regards,
NewLife Hospital"
    
                : $@"Dear {appointment.PatientName},


We regret to inform you that your appointment request at NewLife Hospital could not be approved.

Appointment Details:

Patient Name: {appointment.PatientName}
Doctor: {appointment.Doctor}
Date: {appointment.AppointmentDate:dd MMMM yyyy}
Time: {appointment.AppointmentTime}
Status: Rejected

Please contact NewLife Hospital for further assistance.

Regards,
NewLife Hospital"
            };


        using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(senderEmail, appPassword);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }


}
