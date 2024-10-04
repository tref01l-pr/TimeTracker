using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Options;

namespace TimeTrackerBlazorWitkac.Service.Infrastructure;

public class EmailSenderService : IEmailSender<UserEntity>
{
    private readonly SmtpOptions _smtpOptions;

    public EmailSenderService(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }
    
    public async Task SendConfirmationLinkAsync(UserEntity userEntity, string email, string confirmationLink) =>
        await SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

    public async Task SendPasswordResetLinkAsync(UserEntity userEntity, string email, string resetLink) =>
        await SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    public async Task SendPasswordResetCodeAsync(UserEntity userEntity, string email, string resetCode) =>
        await SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");

    private async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("", _smtpOptions.User));
        emailMessage.To.Add(new MailboxAddress("", email));

        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = htmlMessage
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", _smtpOptions.Port, false);
        await smtp.AuthenticateAsync(_smtpOptions.User, _smtpOptions.Password);
        await smtp.SendAsync(emailMessage);
        await smtp.DisconnectAsync(true);
    }
}