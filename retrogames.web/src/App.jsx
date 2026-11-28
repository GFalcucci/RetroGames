
import { Routes, Route } from 'react-router-dom'

import Layout from './components/Layout'
import RotaProtegida from './components/RotaProtegida'

import Catalogo from './pages/Catalogo'
import DetalheJogo from './pages/DetalheJogo'
import Login from './pages/Login'
import Cadastro from './pages/Cadastro'
import Carrinho from './pages/Carrinho'
import NaoEncontrada from './pages/NaoEncontrada'

import Jogos from './pages/admin/Jogos'
import Plataformas from './pages/admin/Plataformas'
import FormularioJogo from './pages/admin/FormularioJogo'

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        {/* Rotas públicas */}
        <Route index element={<Catalogo />} />
        <Route path="login" element={<Login />} />
        <Route path="cadastro" element={<Cadastro />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />

        {/* Rotas que exigem login (sem sessão, vão ao Login e voltam) */}
        <Route element={<RotaProtegida />}>
          <Route path="carrinho" element={<Carrinho />} />
        </Route>

        {/* Grupo de rotas exclusivas do administrador */}
        <Route element={<RotaProtegida somenteAdministrador />}>
          <Route path="admin">
            <Route path="jogos" element={<Jogos />} />
            <Route path="jogos/novo" element={<FormularioJogo />} />
            <Route path="jogos/:id/editar" element={<FormularioJogo />} />
            <Route path="plataformas" element={<Plataformas />} />
          </Route>
        </Route>

        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
