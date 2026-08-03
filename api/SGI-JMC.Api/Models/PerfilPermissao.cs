namespace SGI_JMC.Api.Models;

// Matriz de permissões: para cada (Perfil, Módulo), se está liberado ou não.
// O perfil "administrador" nunca é consultado aqui — ele sempre tem acesso total,
// verificado diretamente no RequerModuloAttribute.
public class PerfilPermissao
{
    public string Perfil { get; set; } = string.Empty;
    public string ModuloChave { get; set; } = string.Empty;
    public bool Permitido { get; set; }
}
