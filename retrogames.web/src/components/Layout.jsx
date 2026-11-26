
import {
  NavLink,
  Outlet,
  useNavigate,
  useLocation
} from 'react-router-dom'

import useAutenticacao from '../hooks/useAutenticacao'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    sair
  } = useAutenticacao()

  const navigate = useNavigate()
  const location = useLocation()

  const estaNaAdministracao = location.pathname.startsWith('/admin')

  async function tratarSaida() {
    try {
      await sair()
    } finally {
      navigate('/')
    }
  }

  const estiloLink = ({ isActive }) => ({
    color: isActive ? '#F28C28' : '#FFFFFF',
    fontWeight: isActive ? 'bold' : 'normal',
    textDecoration: 'none'
  })

  return (
    <>
      <nav
        className="navbar navbar-expand-lg navbar-dark"
        style={{ backgroundColor: '#701F36' }}
      >
        <div className="container">
          <NavLink
            className="navbar-brand fw-bold d-flex align-items-center gap-2"
            style={{ color: '#FFFFFF' }}
            to="/"
          >
            <img
              src="/simbolo.png"
              alt="Símbolo RetroGames"
              style={{
                width: '50px',
                height: '50px',
                objectFit: 'contain'
              }}
            />

            <span>RetroGames</span>
          </NavLink>

          <div className="navbar-nav ms-auto align-items-lg-center">
            <NavLink
              className="nav-link"
              style={estiloLink}
              to="/"
              end
            >
              Catálogo
            </NavLink>

            {!carregando && (
              <>
                {estaAutenticado && ehAdministrador && (
                  <div className="nav-item dropdown">
                    <button
                      className={`nav-link dropdown-toggle border-0 menu-administracao ${
                        estaNaAdministracao ? 'administracao-ativa' : ''
                      }`}
                      type="button"
                      data-bs-toggle="dropdown"
                      aria-expanded="false"
                    >
                      Administração
                    </button>

                    <ul className="dropdown-menu">
                      <li>
                        <NavLink
                          className="dropdown-item"
                          to="/admin/jogos"
                        >
                          Jogos
                        </NavLink>
                      </li>

                      <li>
                        <NavLink
                          className="dropdown-item"
                          to="/admin/plataformas"
                        >
                          Plataformas
                        </NavLink>
                      </li>
                    </ul>
                  </div>
                )}

                {estaAutenticado ? (
                  <>
                    <span className="navbar-text text-light mx-lg-3">
                      Olá, {usuario.nome}
                    </span>

                    <button
                      type="button"
                      className="btn btn-sm ms-lg-2"
                      style={{
                        backgroundColor: '#F28C28',
                        color: '#FFFFFF',
                        borderColor: '#F28C28'
                      }}
                      onClick={tratarSaida}
                    >
                      Sair
                    </button>
                  </>
                ) : (
                  <NavLink
                    className="nav-link"
                    style={estiloLink}
                    to="/login"
                  >
                    Entrar
                  </NavLink>
                )}
              </>
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
