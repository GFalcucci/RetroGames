
import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'

import { jogoApi } from '../api/servicos'
import { obterMensagemErro } from '../api/erros'
import { formatarPreco } from '../lib/formato'
import { QUANTIDADE_MAXIMA } from '../lib/carrinho'
import useAutenticacao from '../hooks/useAutenticacao'
import useCarrinho from '../hooks/useCarrinho'

import {
  Carregando,
  MensagemErro
} from '../components/MensagensEstado'

function DetalheJogo() {
  const { id } = useParams()

  const { estaAutenticado, ehAdministrador } = useAutenticacao()
  const { itens, adicionar } = useCarrinho()

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

  const esgotado = jogo.estoque <= 0
  const limite = Math.min(QUANTIDADE_MAXIMA, jogo.estoque)
  const noCarrinho =
    itens.find(item => item.idJogo === jogo.id)?.quantidade ?? 0

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

      {!ehAdministrador && (
        <div className="mt-3">
          {esgotado ? (
            <button type="button" className="btn btn-secondary" disabled>
              Esgotado
            </button>
          ) : !estaAutenticado ? (
            <Link
              className="btn btn-primary"
              to={`/login?voltarPara=${encodeURIComponent(`/jogos/${jogo.id}`)}`}
            >
              Entre para comprar
            </Link>
          ) : (
            <>
              <button
                type="button"
                className="btn btn-primary"
                disabled={noCarrinho >= limite}
                onClick={() => adicionar(jogo)}
              >
                Adicionar ao carrinho
              </button>

              {noCarrinho > 0 && (
                <span className="ms-3 text-muted" role="status">
                  {noCarrinho} no carrinho ·{' '}
                  <Link to="/carrinho">Ver carrinho</Link>
                  {noCarrinho >= limite &&
                    ` · limite de ${limite} por pedido`}
                </span>
              )}
            </>
          )}
        </div>
      )}
    </section>
  )
}

export default DetalheJogo
