import { useEffect } from 'react'
import { Collapse, Dropdown } from 'bootstrap'
import {
  NavLink,
  Outlet,
  useNavigate,
  useLocation
} from 'react-router-dom'

import useAutenticacao from '../hooks/useAutenticacao'
import useCarrinho from '../hooks/useCarrinho'
import IconeCarrinho from './IconeCarrinho'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    sair
  } = useAutenticacao()

  const { totalItens } = useCarrinho()

  const navigate = useNavigate()
  const location = useLocation()

  const estaNaAdministracao = location.pathname.startsWith('/admin')
  const ehCliente = estaAutenticado && !ehAdministrador

  // Fecha o menu principal ao trocar de página.
  useEffect(() => {
    const menu = document.getElementById('menuPrincipal')

    if (menu?.classList.contains('show')) {
      Collapse.getInstance(menu)?.hide()
    }
  }, [location.pathname])

  // Inicializa o menu suspenso de administração.
  useEffect(() => {
    if (carregando) return

    document
      .querySelectorAll('[data-bs-toggle="dropdown"]')
      .forEach((elemento) => Dropdown.getOrCreateInstance(elemento))
  }, [carregando, estaAutenticado, ehAdministrador])

  // Realiza o logout e redireciona para o catálogo.
  async function tratarSaida() {
    try {
      await sair()
    } finally {
      navigate('/')
    }
  }

  // Estilo dos links da navegação.
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

          {/* Logo e nome da loja */}
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

          {/* Botão do menu principal em telas pequenas */}
          <button
            className="navbar-toggler ms-auto order-2 d-lg-none"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#menuPrincipal"
            aria-controls="menuPrincipal"
            aria-expanded="false"
            aria-label="Abrir menu"
          >
            <span className="navbar-toggler-icon" />
          </button>

          {/* Menu principal */}
          <div
            className="collapse navbar-collapse order-3 order-lg-2"
            id="menuPrincipal"
          >
            <div className="navbar-nav ms-lg-auto align-items-lg-center">

              {/* Saudação: primeiro item, somente em telas pequenas */}
              {!carregando && estaAutenticado && (
                <span
                  className="nav-link fw-bold d-lg-none"
                  style={{ color: '#FFFFFF' }}
                >
                  Olá, {usuario?.nome}
                </span>
              )}

              {/* Catálogo */}
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
                  {/* Menu de administração */}
                  {estaAutenticado && ehAdministrador && (
                    <div className="nav-item dropdown">

                      <button
                        className={`nav-link dropdown-toggle border-0 menu-administracao ${
                          estaNaAdministracao
                            ? 'administracao-ativa'
                            : ''
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

                  {/* Pedidos do cliente */}
                  {ehCliente && (
                    <NavLink
                      className="nav-link"
                      style={estiloLink}
                      to="/meus-pedidos"
                    >
                      Meus pedidos
                    </NavLink>
                  )}

                  {/* Sair: último item, somente em telas pequenas */}
                  {estaAutenticado && (
                    <button
                      type="button"
                      className="btn btn-sm fw-bold my-2 align-self-start d-lg-none"
                      style={{
                        backgroundColor: '#F28C28',
                        color: '#FFFFFF',
                        borderColor: '#F28C28'
                      }}
                      onClick={tratarSaida}
                    >
                      Sair
                    </button>
                  )}
                </>
              )}

            </div>
          </div>

          {/* Ações do usuário: fora do menu principal */}
          <div className="d-flex align-items-center ms-auto ms-lg-4 order-1 order-lg-3">

            {/* Carrinho: permanece sempre visível */}
            {!carregando && ehCliente && (
              <NavLink
                className="nav-link d-flex align-items-center gap-1 me-3"
                style={estiloLink}
                to="/carrinho"
                title="Carrinho"
                aria-label={
                  totalItens > 0
                    ? `Carrinho, ${totalItens} itens`
                    : 'Carrinho'
                }
              >
                <IconeCarrinho />

                {totalItens > 0 && (
                  <span
                    className="badge rounded-pill"
                    style={{ backgroundColor: '#F28C28' }}
                  >
                    {totalItens}
                  </span>
                )}
              </NavLink>
            )}

            {!carregando && (
              <>
                {estaAutenticado ? (
                  /* Saudação e Sair juntos em uma caixa laranja (telas grandes) */
                  <div
                    className="d-none d-lg-flex align-items-center gap-3 rounded px-3 py-1"
                    style={{ backgroundColor: '#F28C28' }}
                  >
                    <span
                      className="fw-bold"
                      style={{ color: '#FFFFFF' }}
                    >
                      Olá, {usuario?.nome}
                    </span>

                    <span
                      style={{
                        width: '1px',
                        height: '20px',
                        backgroundColor: 'rgba(255, 255, 255, 0.6)'
                      }}
                      aria-hidden="true"
                    />

                    <button
                      type="button"
                      className="btn btn-link p-0 fw-bold text-decoration-none"
                      style={{ color: '#FFFFFF' }}
                      onClick={tratarSaida}
                    >
                      Sair
                    </button>
                  </div>
                ) : (
                  <NavLink
                    className="btn btn-sm"
                    style={{
                      backgroundColor: '#F28C28',
                      color: '#FFFFFF',
                      borderColor: '#F28C28'
                    }}
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

      {/* Conteúdo das páginas */}
      <main className="container py-4">
        <Outlet />
      </main>
    </>
  )
}

export default Layout