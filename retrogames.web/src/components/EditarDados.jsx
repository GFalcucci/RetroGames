import { useEffect, useRef, useState } from 'react'
import { Collapse } from 'bootstrap'
import {
  NavLink,
  Outlet,
  useNavigate,
  useLocation
} from 'react-router-dom'

import useAutenticacao from '../hooks/useAutenticacao'
import useCarrinho from '../hooks/useCarrinho'
import IconeCarrinho from './IconeCarrinho'
import IconePessoa from './IconePessoa'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    ehCliente,
    sair
  } = useAutenticacao()

  const { totalItens } = useCarrinho()
  const navigate = useNavigate()
  const location = useLocation()

  const menuPrincipalRef = useRef(null)
  const menuUsuarioRef = useRef(null)

  const [menuUsuarioAberto, setMenuUsuarioAberto] = useState(false)

  useEffect(() => {
    if (!menuPrincipalRef.current) return

    const collapse = new Collapse(menuPrincipalRef.current, {
      toggle: false
    })

    return () => collapse.dispose()
  }, [])

  useEffect(() => {
    function fecharMenuUsuario(event) {
      if (
        menuUsuarioRef.current &&
        !menuUsuarioRef.current.contains(event.target)
      ) {
        setMenuUsuarioAberto(false)
      }
    }

    document.addEventListener('mousedown', fecharMenuUsuario)

    return () => {
      document.removeEventListener('mousedown', fecharMenuUsuario)
    }
  }, [])

  useEffect(() => {
    setMenuUsuarioAberto(false)
  }, [location.pathname])

  function tratarSaida() {
    setMenuUsuarioAberto(false)
    sair()
    navigate('/')
  }

  return (
    <>
      <header>
        <nav
          className="navbar navbar-expand-lg"
          style={{ backgroundColor: '#701F36' }}
        >
          <div className="container">
            <NavLink
              to="/"
              className="navbar-brand fw-bold"
              style={{ color: '#FFFFFF' }}
            >
              RetroGames
            </NavLink>

            <div className="d-flex align-items-center gap-3 order-lg-last">
              <NavLink
                to="/carrinho"
                className="nav-link position-relative d-flex align-items-center"
                style={{ color: '#FFFFFF' }}
                aria-label="Abrir carrinho"
              >
                <IconeCarrinho tamanho={28} />

                {totalItens > 0 && (
                  <span
                    className="position-absolute top-0 start-100 translate-middle badge rounded-pill"
                    style={{
                      backgroundColor: '#F28C28',
                      color: '#FFFFFF'
                    }}
                  >
                    {totalItens}
                  </span>
                )}
              </NavLink>

              {!carregando && (
                <div
                  className="position-relative"
                  ref={menuUsuarioRef}
                >
                  <button
                    type="button"
                    className="nav-link border-0"
                    style={{
                      color: '#FFFFFF',
                      background: 'transparent',
                      padding: 0,
                      cursor: 'pointer'
                    }}
                    onClick={() =>
                      setMenuUsuarioAberto(aberto => !aberto)
                    }
                    aria-label="Abrir menu do usuário"
                    aria-expanded={menuUsuarioAberto}
                  >
                    <IconePessoa tamanho={28} />
                  </button>

                  {menuUsuarioAberto && (
                    <div
                      className="dropdown-menu show"
                      style={{
                        position: 'absolute',
                        top: '100%',
                        right: 0,
                        width: '220px',
                        maxWidth: 'calc(100vw - 24px)',
                        marginTop: '8px',
                        zIndex: 1050
                      }}
                    >
                      {estaAutenticado ? (
                        <>
                          <div className="dropdown-item-text fw-bold">
                            Olá, {usuario?.nome}
                          </div>

                          <hr className="dropdown-divider" />

                          <NavLink
                            to="/minha-conta"
                            className="dropdown-item"
                            onClick={() => setMenuUsuarioAberto(false)}
                          >
                            Editar meus dados
                          </NavLink>

                          <button
                            type="button"
                            className="dropdown-item"
                            onClick={tratarSaida}
                          >
                            Sair
                          </button>
                        </>
                      ) : (
                        <NavLink
                          to="/login"
                          className="dropdown-item"
                          onClick={() => setMenuUsuarioAberto(false)}
                        >
                          Entrar
                        </NavLink>
                      )}
                    </div>
                  )}
                </div>
              )}
            </div>

            <button
              className="navbar-toggler ms-2"
              type="button"
              aria-label="Abrir menu de navegação"
              onClick={() => {
                if (menuPrincipalRef.current) {
                  const collapse = Collapse.getOrCreateInstance(
                    menuPrincipalRef.current
                  )
                  collapse.toggle()
                }
              }}
            >
              <span className="navbar-toggler-icon" />
            </button>

            <div
              className="collapse navbar-collapse"
              id="menuPrincipal"
              ref={menuPrincipalRef}
            >
              <ul className="navbar-nav ms-auto">
                <li className="nav-item">
                  <NavLink
                    to="/catalogo"
                    className="nav-link"
                    style={{ color: '#FFFFFF' }}
                  >
                    Catálogo
                  </NavLink>
                </li>

                {estaAutenticado && ehCliente && (
                  <li className="nav-item">
                    <NavLink
                      to="/meus-pedidos"
                      className="nav-link"
                      style={{ color: '#FFFFFF' }}
                    >
                      Meus pedidos
                    </NavLink>
                  </li>
                )}

                {estaAutenticado && ehAdministrador && (
                  <>
                    <li className="nav-item">
                      <NavLink
                        to="/admin/jogos"
                        className="nav-link"
                        style={{ color: '#FFFFFF' }}
                      >
                        Administração
                      </NavLink>
                    </li>

                    <li className="nav-item">
                      <NavLink
                        to="/admin/plataformas"
                        className="nav-link"
                        style={{ color: '#FFFFFF' }}
                      >
                        Plataformas
                      </NavLink>
                    </li>
                  </>
                )}
              </ul>
            </div>
          </div>
        </nav>
      </header>

      <main>
        <Outlet />
      </main>
    </>
  )
}

export default Layout