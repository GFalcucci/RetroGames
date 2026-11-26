import { Link } from 'react-router-dom'

function NaoEncontrada() {
  return (
    <div className="text-center py-5">
      <h1>404</h1>
      <p className="lead">Página não encontrada.</p>

      <Link to="/" className="btn btn-primary">
        Voltar ao catálogo
      </Link>
    </div>
  )
}

export default NaoEncontrada