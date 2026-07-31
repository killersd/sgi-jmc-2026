import { useAuth } from "../context/AuthContext";
import "./DashboardPage.css";

export default function DashboardPage() {
  const { user } = useAuth();
  const primeiroNome = user?.nome?.split(" ")[0] ?? "";

  return (
    <div className="dashboard">
      <header className="page-header">
        <h1>Olá, {primeiroNome}</h1>
        <p>Visão geral do sistema de gestão institucional.</p>
      </header>

      <section className="dashboard__grid">
        <a className="dashboard__card" href="/alunos">
          <span className="dashboard__card-icon">▤</span>
          <div>
            <h3>Alunos</h3>
            <p>Consultar, atualizar e excluir dados de alunos.</p>
          </div>
        </a>
        <a className="dashboard__card" href="/declaracoes">
          <span className="dashboard__card-icon">▥</span>
          <div>
            <h3>Declarações</h3>
            <p>Buscar aluno pelo código e emitir declaração de frequência.</p>
          </div>
        </a>
        <a className="dashboard__card" href="/oficios">
          <span className="dashboard__card-icon">✉</span>
          <div>
            <h3>Ofícios</h3>
            <p>Emitir ofício geral.</p>
          </div>
        </a>
        <a className="dashboard__card" href="/horarios">
          <span className="dashboard__card-icon">▦</span>
          <div>
            <h3>Horários</h3>
            <p>Grade semanal de aulas por professor.</p>
          </div>
        </a>
        <a className="dashboard__card" href="/professores">
          <span className="dashboard__card-icon">👤</span>
          <div>
            <h3>Professores</h3>
            <p>Cadastro de professores e disciplinas que lecionam.</p>
          </div>
        </a>
        <a className="dashboard__card" href="/turmas">
          <span className="dashboard__card-icon">🏷</span>
          <div>
            <h3>Turmas</h3>
            <p>Catálogo de turmas usado nos horários.</p>
          </div>
        </a>
        <a className="dashboard__card" href="/ocorrencias">
          <span className="dashboard__card-icon">⚠</span>
          <div>
            <h3>Advertências e Suspensões</h3>
            <p>Buscar aluno pelo código e emitir a advertência ou suspensão.</p>
          </div>
        </a>
      </section>
    </div>
  );
}
