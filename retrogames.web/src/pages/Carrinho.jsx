import { useState } from 'react'
import { Link } from 'react-router-dom'

import useCarrinho from '../hooks/useCarrinho'
import { pedidoApi } from '../api/servicos'
import { obterMensagemErro } from '../api/erros'
import { formatarPreco, formatarDataHora } from '../lib/formato'
import { EstadoVazio } from '../components/MensagensEstado'
import { QUANTIDADE_MAXIMA } from '../lib/carrinho'

function Carrinho() {
  const { itens, total, alterarQuantidade, remover, limpar } = useCarrinho()

  const [finalizando, setFinalizando] = useState(false)
  const [erro, setErro] = useState('')
  const [pedido, setPedido] = useState(null)

  async function finalizarCompra() {
    try {
      setFinalizando(true)
      setErro('')

      // Só o id e a quantidade seguem para a API. O preço vem do banco.
      const criado = await pedidoApi.inserir({
        itens: itens.map(item => ({
          idJogo: item.idJogo,
          quantidade: item.quantidade
        }))
      })

      limpar()
      setPedido(criado)
    } catch (erro) {
      setErro(obterMensagemErro(erro))
    } finally {
      setFinalizando(false)
    }
  }

  if (pedido) {
    return (
      <section className="text-center py-4">
        <div className="alert alert-success d-inline-block" role="status">
          <h1 className="h4">Compra realizada!</h1>

          <p className="mb-1">
            Pedido nº {pedido.id} · {formatarDataHora(pedido.dataPedido)}
          </p>

          <p className="mb-0">
            Total: <strong>{formatarPreco(pedido.total)}</strong> ·
            Situação: Pendente
          </p>
        </div>

        <div>
          <Link to="/" className="btn btn-primary">
            Voltar ao catálogo
          </Link>
        </div>
      </section>
    )
  }

  if (itens.length === 0) {
    return (
      <EstadoVazio titulo="Seu carrinho está vazio.">
        <p>Escolha um jogo no catálogo para começar.</p>

        <Link to="/" className="btn btn-primary">
          Ver o catálogo
        </Link>
      </EstadoVazio>
    )
  }

  return (
    <section>
      <h1 className="mb-4">Carrinho</h1>

      {erro && (
        <div className="alert alert-danger" role="alert">
          {erro}
        </div>
      )}

      <div className="table-responsive">
        <table className="table align-middle">
          <thead>
            <tr>
              <th>Jogo</th>
              <th className="text-end">Preço</th>
              <th>Quantidade</th>
              <th className="text-end">Subtotal</th>
              <th>
                <span className="visually-hidden">Ações</span>
              </th>
            </tr>
          </thead>

          <tbody>
            {itens.map(item => {
              const maximo = Math.min(QUANTIDADE_MAXIMA, item.estoque)

              return (
                <tr key={item.idJogo}>
                  <td>
                    <Link to={`/jogos/${item.idJogo}`}>{item.nome}</Link>
                    <div className="text-muted small">
                      {item.nomePlataforma}
                    </div>
                  </td>

                  <td className="text-end">{formatarPreco(item.preco)}</td>

                  <td>
                    <select
                      className="form-select form-select-sm"
                      style={{ width: '5rem' }}
                      aria-label={`Quantidade de ${item.nome}`}
                      value={item.quantidade}
                      disabled={finalizando}
                      onChange={(evento) =>
                        alterarQuantidade(item.idJogo, evento.target.value)
                      }
                    >
                      {Array.from({ length: maximo }, (_, i) => i + 1).map(
                        quantidade => (
                          <option key={quantidade} value={quantidade}>
                            {quantidade}
                          </option>
                        )
                      )}
                    </select>
                  </td>

                  <td className="text-end">
                    {formatarPreco(item.preco * item.quantidade)}
                  </td>

                  <td className="text-end">
                    <button
                      type="button"
                      className="btn btn-outline-danger btn-sm"
                      disabled={finalizando}
                      onClick={() => remover(item.idJogo)}
                    >
                      Remover
                    </button>
                  </td>
                </tr>
              )
            })}
          </tbody>

          <tfoot>
            <tr>
              <th colSpan={3} className="text-end">Total</th>
              <th className="text-end">{formatarPreco(total)}</th>
              <th />
            </tr>
          </tfoot>
        </table>
      </div>

      <div className="d-flex flex-column flex-sm-row justify-content-between gap-2">
        <Link to="/" className="btn btn-outline-secondary">
          Continuar comprando
        </Link>

        <button
          type="button"
          className="btn btn-primary"
          disabled={finalizando}
          onClick={finalizarCompra}
        >
          {finalizando ? 'Finalizando...' : 'Finalizar compra'}
        </button>
      </div>
    </section>
  )
}

export default Carrinho
