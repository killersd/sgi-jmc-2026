import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import "./AppLayout.css";

const navItems = [
  { to: "/", label: "Painel", icon: "◧", end: true },
  { to: "/alunos", label: "Alunos", icon: "▤", end: false },
  { to: "/declaracoes", label: "Declarações", icon: "▥", end: false },
  { to: "/oficios", label: "Ofícios", icon: "✉", end: false },
  { to: "/horarios", label: "Horários", icon: "▦", end: false },
  { to: "/professores", label: "Professores", icon: "👤", end: false },
  { to: "/turmas", label: "Turmas", icon: "🏷", end: false },
];

export default function AppLayout() {
  const { user, logout } = useAuth();
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

  return (
    <div className="app-shell">
      <aside className="app-sidebar">
        <div className="app-sidebar__brand">
          <span className="app-sidebar__mark">JMC</span>
          <span className="app-sidebar__name">SGI · JMC</span>
        </div>

        <nav className="app-sidebar__nav">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.end}
              className={({ isActive }) =>
                "app-sidebar__link" + (isActive ? " app-sidebar__link--active" : "")
              }
            >
              <span className="app-sidebar__icon" aria-hidden>{item.icon}</span>
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="app-sidebar__footer">
          <div className="app-sidebar__user">
            <span className="app-sidebar__avatar">{iniciais}</span>
            <div>
              <div className="app-sidebar__user-name">{user?.nome}</div>
              <div className="app-sidebar__user-role">
                {user?.roles.includes("administrador") ? "Administrador" : "Usuário"}
              </div>
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
