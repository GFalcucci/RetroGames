import { NavLink, Outlet } from 'react-router-dom'

function Layout() {
  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <NavLink className="navbar-brand" to="/">
            RetroGames
          </NavLink>

          <div className="navbar-nav">
            <NavLink
              className={({ isActive }) =>
                `nav-link ${isActive ? 'active' : ''}`
              }
              to="/"
            >
              Catálogo
            </NavLink>
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