using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SGI_JMC.Api.Services;

public interface IEmailService
{
    Task EnviarAsync(string destinatario, string assunto, string corpoHtml);
}

public class EmailService : IEmailService
{
    private readonly SmtpOptions _opcoes;

    public EmailService(IOptions<SmtpOptions> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public async Task EnviarAsync(string destinatario, string assunto, string corpoHtml)
    {
        var mensagem = new MimeMessage();
        mensagem.From.Add(new MailboxAddress(_opcoes.RemetenteNome, _opcoes.RemetenteEmail));
        mensagem.To.Add(MailboxAddress.Parse(destinatario));
        mensagem.Subject = assunto;
        mensagem.Body = new BodyBuilder { HtmlBody = corpoHtml }.ToMessageBody();

        using var cliente = new SmtpClient();
        // Em algumas redes (proxy/firewall corporativo) a checagem de revogação (CRL/OCSP)
        // do certificado não consegue ser concluída; desabilitamos só essa checagem,
        // mantendo a validação normal da cadeia/hostname do certificado.
        cliente.CheckCertificateRevocation = false;
        try
        {
            await cliente.ConnectAsync(_opcoes.Host, _opcoes.Port, SecureSocketOptions.StartTls);
            await cliente.AuthenticateAsync(_opcoes.Usuario, _opcoes.Senha);
            await cliente.SendAsync(mensagem);
        }
        finally
        {
            if (cliente.IsConnected)
                await cliente.DisconnectAsync(true);
        }
    }
}
