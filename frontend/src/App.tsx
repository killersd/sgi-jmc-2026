import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import RequireAuth from "./routes/RequireAuth";
import AppLayout from "./layouts/AppLayout";
import LoginPage from "./pages/LoginPage";
import DashboardPage from "./pages/DashboardPage";
import AlunosPage from "./pages/AlunosPage";
import DeclaracoesPage from "./pages/DeclaracoesPage";
import OficiosPage from "./pages/OficiosPage";
import HorariosPage from "./pages/HorariosPage";
import HorarioEditorPage from "./pages/HorarioEditorPage";
import ProfessoresPage from "./pages/ProfessoresPage";
import TurmasPage from "./pages/TurmasPage";

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route
            path="/"
            element={
              <RequireAuth>
                <AppLayout />
              </RequireAuth>
            }
          >
            <Route index element={<DashboardPage />} />
            <Route path="alunos" element={<AlunosPage />} />
            <Route path="declaracoes" element={<DeclaracoesPage />} />
            <Route path="oficios" element={<OficiosPage />} />
            <Route path="horarios" element={<HorariosPage />} />
            <Route path="horarios/:id" element={<HorarioEditorPage />} />
            <Route path="professores" element={<ProfessoresPage />} />
            <Route path="turmas" element={<TurmasPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
