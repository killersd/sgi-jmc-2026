namespace SGI_JMC.Api.Authorization;

// Catálogo fixo dos módulos do sistema, usado tanto para checar permissão
// quanto para montar a matriz de configuração em Administração.
public static class Modulos
{
    public const string Alunos = "alunos";
    public const string Declaracoes = "declaracoes";
    public const string Oficios = "oficios";
    public const string Horarios = "horarios";
    public const string Professores = "professores";
    public const string Turmas = "turmas";
    public const string Ocorrencias = "ocorrencias";

    public static readonly (string Chave, string Nome)[] Todos =
    {
        (Alunos, "Alunos"),
        (Declaracoes, "Declarações"),
        (Oficios, "Ofícios"),
        (Horarios, "Horários"),
        (Professores, "Professores"),
        (Turmas, "Turmas"),
        (Ocorrencias, "Advertências e Suspensões"),
    };
}
