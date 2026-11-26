import { useParams } from 'react-router-dom'

function DetalheJogo() {
  const { id } = useParams()

  return (
    <h1>Detalhe do jogo {id}</h1>
  )
}

export default DetalheJogo