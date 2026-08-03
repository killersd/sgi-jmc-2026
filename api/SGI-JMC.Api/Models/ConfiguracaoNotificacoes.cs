namespace SGI_JMC.Api.Models;

// Linha única (Id = 1) com configurações de notificação por e-mail, editável em Administração.
public class ConfiguracaoNotificacoes
{
    public int Id { get; set; }
    public string? EmailNotificacaoTransferencia { get; set; }
}
