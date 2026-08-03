import { useAuth } from "../context/AuthContext";
import "./DashboardPage.css";

const cards = [
  { href: "/alunos", modulo: "alunos", icon: "▤", titulo: "Alunos", texto: "Consultar, matricular e atualizar dados de alunos." },
  { href: "/declaracoes", modulo: "declaracoes", icon: "▥", titulo: "Declarações", texto: "Buscar aluno pelo código e emitir declaração de frequência em PDF." },
  { href: "/oficios", modulo: "oficios", icon: "✉", titulo: "Ofícios", texto: "Emitir ofício geral (carta oficial) em PDF." },
  { href: "/horarios", modulo: "horarios", icon: "▦", titulo: "Horários", texto: "Grade semanal de aulas por professor, fácil de editar." },
  { href: "/professores", modulo: "professores", icon: "👤", titulo: "Professores", texto: "Cadastro de professores e disciplinas que lecionam." },
  { href: "/turmas", modulo: "turmas", icon: "🏷", titulo: "Turmas", texto: "Catálogo de turmas usado nos horários." },
  { href: "/ocorrencias", modulo: "ocorrencias", icon: "⚠", titulo: "Advertências e Suspensões", texto: "Buscar aluno pelo código e emitir o documento em PDF." },
];

export default function DashboardPage() {
  const { user, isAdmin, isDiretor, podeAcessarModulo } = useAuth();
  const primeiroNome = user?.nome?.split(" ")[0] ?? "";
  const cardsVisiveis = cards.filter((c) => podeAcessarModulo(c.modulo));

  return (
    <div className="dashboard">
      <header className="page-header">
        <h1>Olá, {primeiroNome}</h1>
        <p>Visão geral do sistema de gestão institucional.</p>
      </header>

      <section className="dashboard__grid">
        {cardsVisiveis.map((card) => (
          <a className="dashboard__card" href={card.href} key={card.href}>
            <span className="dashboard__card-icon">{card.icon}</span>
            <div>
              <h3>{card.titulo}</h3>
              <p>{card.texto}</p>
            </div>
          </a>
        ))}

        {(isAdmin || isDiretor) && (
          <a className="dashboard__card" href="/administracao">
            <span className="dashboard__card-icon">⚙</span>
            <div>
              <h3>Administração</h3>
              <p>Controle de acesso por perfil e gestão de usuários.</p>
            </div>
          </a>
        )}

        {cardsVisiveis.length === 0 && !isAdmin && !isDiretor && (
          <p className="horarios-page__estado">Seu perfil ainda não tem módulos liberados. Fale com a direção.</p>
        )}
      </section>
    </div>
  );
}
