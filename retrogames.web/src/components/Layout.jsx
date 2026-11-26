import { NavLink, Outlet, useNavigate } from 'react-router-dom'

import useAutenticacao from '../hooks/useAutenticacao'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    sair
  } = useAutenticacao()

  const navigate = useNavigate()

  async function tratarSaida() {
    try {
      await sair()
    } finally {
      navigate('/')
    }
  }

  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <NavLink className="navbar-brand" to="/">
            RetroGames
          </NavLink>

          <div className="navbar-nav ms-auto align-items-lg-center">
            <NavLink
              className={({ isActive }) =>
                `nav-link ${isActive ? 'active' : ''}`
              }
              to="/"
            >
              Catálogo
            </NavLink>

            {!carregando && (
              estaAutenticado ? (
                <>
                  <span className="navbar-text text-light mx-lg-3">
                    Olá, {usuario.nome}
                  </span>

                  <button
                    type="button"
                    className="btn btn-outline-light btn-sm ms-lg-2"
                    onClick={tratarSaida}
                  >
                    Sair
                  </button>
                </>
              ) : (
                <NavLink
                  className={({ isActive }) =>
                    `nav-link ${isActive ? 'active' : ''}`
                  }
                  to="/login"
                >
                  Entrar
                </NavLink>
              )
            )}
          </div>
        </div>
      </nav>

      <main className="container py-4">
        <Outlet />
      </main>
    </>
  )
}

export default Layout