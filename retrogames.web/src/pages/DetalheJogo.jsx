
import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'

import { jogoApi } from '../api/servicos'
import { obterMensagemErro } from '../api/erros'
import { formatarPreco } from '../lib/formato'

import {
  Carregando,
  MensagemErro
} from '../components/MensagensEstado'

function DetalheJogo() {
  const { id } = useParams()

  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let cancelado = false

    async function carregarJogo() {
      try {
        setCarregando(true)
        setErro('')

        const dados = await jogoApi.buscarPorId(id)

        if (!cancelado) {
          setJogo(dados)
        }
      } catch (erro) {
        if (!cancelado) {
          setErro(obterMensagemErro(erro))
        }
      } finally {
        if (!cancelado) {
          setCarregando(false)
        }
      }
    }

    carregarJogo()

    return () => {
      cancelado = true
    }
  }, [id, tentativa])

  if (carregando) {
    return <Carregando mensagem="Carregando detalhes do jogo..." />
  }

  if (erro) {
    return (
      <MensagemErro
        mensagem={erro}
        aoTentarNovamente={() => setTentativa(valor => valor + 1)}
      />
    )
  }

  if (!jogo) {
    return null
  }

  return (
    <section>
      <h1 className="mb-4">{jogo.nome}</h1>

      <div className="card">
        <div className="card-body">
          <p>
            <strong>Plataforma:</strong> {jogo.nomePlataforma}
          </p>

          <p>
            <strong>Ano de lançamento:</strong> {jogo.anoLancamento}
          </p>

          <p>
            <strong>Condição:</strong> {jogo.descricaoCondicao}
          </p>

          <p>
            <strong>Gêneros:</strong> {jogo.generos.join(', ')}
          </p>

          <p>
            <strong>Preço:</strong> {formatarPreco(jogo.preco)}
          </p>

          <p>
            <strong>Estoque:</strong>{' '}
            {jogo.estoque > 0 ? jogo.estoque : 'Esgotado'}
          </p>

          {jogo.descricao && (
            <p>
              <strong>Descrição:</strong> {jogo.descricao}
            </p>
          )}
        </div>
      </div>
    </section>
  )
}

export default DetalheJogo
