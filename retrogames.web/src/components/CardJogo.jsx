import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

function CardJogo({ jogo }) {
  return (
    <div className="card h-100 shadow-sm">
      <div className="card-body">
        <h2 className="card-title h5">{jogo.nome}</h2>

        <p className="text-muted mb-1">
          {jogo.nomePlataforma} · {jogo.anoLancamento}
        </p>

        <p className="mb-1">
          <strong>Condição:</strong> {jogo.descricaoCondicao}
        </p>

        <p className="mb-1">
          <strong>Gêneros:</strong> {jogo.generos.join(', ')}
        </p>

        <p className="mb-1">
          <strong>Preço:</strong> {formatarPreco(jogo.preco)}
        </p>

        <p className="mb-3">
          {jogo.estoque > 0
            ? `Estoque: ${jogo.estoque}`
            : 'Esgotado'}
        </p>

        <Link
          to={`/jogos/${jogo.id}`}
          className="btn btn-primary"
        >
          Ver detalhes
        </Link>
      </div>
    </div>
  )
}

export default CardJogo