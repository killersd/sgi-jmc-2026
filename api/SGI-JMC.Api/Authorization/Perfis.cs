namespace SGI_JMC.Api.Authorization;

// Perfis fixos do sistema. "Administrador" sempre tem acesso total a tudo
// (nunca é bloqueado por permissão de módulo).
public static class Perfis
{
    public const string Administrador = "administrador";
    public const string Diretor = "diretor";
    public const string Secretario = "secretario";
    public const string OficialAdministrativo = "oficial_administrativo";

    public static readonly (string Chave, string Nome)[] Todos =
    {
        (Administrador, "Administrador"),
        (Diretor, "Diretor Escolar"),
        (Secretario, "Secretário Escolar"),
        (OficialAdministrativo, "Oficial Administrativo"),
    };

    // Perfis que o Diretor pode configurar (não pode mexer no próprio nem no de Administrador)
    public static readonly string[] ConfiguraveisPeloDiretor = { Secretario, OficialAdministrativo };

    // Perfis que o Administrador pode configurar (todos exceto o próprio — Administrador sempre tem tudo)
    public static readonly string[] ConfiguraveisPeloAdministrador = { Diretor, Secretario, OficialAdministrativo };
}
