
import { Navigate, Outlet, useLocation } from 'react-router-dom'

import useAutenticacao from '../hooks/useAutenticacao'
import { Carregando } from './MensagensEstado'

function RotaProtegida({ somenteAdministrador = false }) {
  const {
    carregando,
    estaAutenticado,
    ehAdministrador
  } = useAutenticacao()

  const localizacao = useLocation()

  if (carregando) {
    return <Carregando />
  }

  if (!estaAutenticado) {
    const voltarPara = encodeURIComponent(
      localizacao.pathname + localizacao.search
    )

    return (
      <Navigate
        to={`/login?voltarPara=${voltarPara}`}
        replace
      />
    )
  }

  if (somenteAdministrador && !ehAdministrador) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}

export default RotaProtegida
