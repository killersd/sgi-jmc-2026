import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import "./AppLayout.css";

const navItems = [
  { to: "/alunos", label: "Alunos", icon: "▤", modulo: "alunos" },
  { to: "/declaracoes", label: "Declarações", icon: "▥", modulo: "declaracoes" },
  { to: "/oficios", label: "Ofícios", icon: "✉", modulo: "oficios" },
  { to: "/horarios", label: "Horários", icon: "▦", modulo: "horarios" },
  { to: "/professores", label: "Professores", icon: "👤", modulo: "professores" },
  { to: "/turmas", label: "Turmas", icon: "🏷", modulo: "turmas" },
  { to: "/ocorrencias", label: "Advertências/Suspensões", icon: "⚠", modulo: "ocorrencias" },
];

const nomesPerfis: Record<string, string> = {
  administrador: "Administrador",
  diretor: "Diretor Escolar",
  secretario: "Secretário Escolar",
  oficial_administrativo: "Oficial Administrativo",
};

export default function AppLayout() {
  const { user, logout, isAdmin, isDiretor, podeAcessarModulo } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login", { replace: true });
  }

  const iniciais = (user?.nome ?? "?")
    .split(" ")
    .map((parte) => parte[0])
    .slice(0, 2)
    .join("")
    .toUpperCase();

  const nomePerfil = user?.roles.map((r) => nomesPerfis[r] ?? r).join(", ") ?? "";
  const itensVisiveis = navItems.filter((item) => podeAcessarModulo(item.modulo));

  return (
    <div className="app-shell">
      <aside className="app-sidebar">
        <div className="app-sidebar__brand">
          <span className="app-sidebar__mark">SGI</span>
          <span className="app-sidebar__name">Escola Estadual João de Mattos Carvalho</span>
        </div>

        <nav className="app-sidebar__nav">
          <NavLink
            to="/"
            end
            className={({ isActive }) =>
              "app-sidebar__link" + (isActive ? " app-sidebar__link--active" : "")
            }
          >
            <span className="app-sidebar__icon" aria-hidden>◧</span>
            Painel
          </NavLink>

          {itensVisiveis.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                "app-sidebar__link" + (isActive ? " app-sidebar__link--active" : "")
              }
            >
              <span className="app-sidebar__icon" aria-hidden>{item.icon}</span>
              {item.label}
            </NavLink>
          ))}

          {(isAdmin || isDiretor) && (
            <NavLink
              to="/administracao"
              className={({ isActive }) =>
                "app-sidebar__link" + (isActive ? " app-sidebar__link--active" : "")
              }
            >
              <span className="app-sidebar__icon" aria-hidden>⚙</span>
              Administração
            </NavLink>
          )}
        </nav>

        <div className="app-sidebar__footer">
          <div className="app-sidebar__user">
            <span className="app-sidebar__avatar">{iniciais}</span>
            <div>
              <div className="app-sidebar__user-name">{user?.nome}</div>
              <div className="app-sidebar__user-role">{nomePerfil}</div>
            </div>
          </div>
          <button className="app-sidebar__logout" onClick={handleLogout}>
            Sair
          </button>
        </div>
      </aside>

      <div className="app-content">
        <Outlet />
      </div>
    </div>
  );
}
