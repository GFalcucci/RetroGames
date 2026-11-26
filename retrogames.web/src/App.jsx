import { Routes, Route } from 'react-router-dom'

import Layout from './components/Layout'
import Catalogo from './pages/Catalogo'
import DetalheJogo from './pages/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App