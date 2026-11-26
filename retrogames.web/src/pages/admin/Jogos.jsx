
import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'
import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'
import Paginacao from '../../components/Paginacao'

function Jogos() {
  const [jogos, setJogos] = useState([])
  const [pagina, setPagina] = useState(1)
  const [totalPaginas, setTotalPaginas] = useState(1)
  const [busca, setBusca] = useState('')
  const [buscaAplicada, setBuscaAplicada] = useState('')
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)
  const [erroAcao, setErroAcao] = useState(null)
  const [atualizacao, setAtualizacao] = useState(0)

  useEffect(() => {
    async function carregarJogos() {
      setCarregando(true)
      setErro(null)

      try {
        const dados = await jogoApi.listar({
          busca: buscaAplicada || undefined,
          incluirInativos: true,
          pagina,
          tamanhoPagina: 10
        })

        setJogos(dados?.itens ?? [])
        setTotalPaginas(dados?.totalPaginas ?? 1)
      } catch (erro) {
        setErro(obterMensagemErro(erro))
      } finally {
        setCarregando(false)
      }
    }

    carregarJogos()
  }, [pagina, buscaAplicada, atualizacao])

  function pesquisar(evento) {
    evento.preventDefault()
    setPagina(1)
    setBuscaAplicada(busca.trim())
  }

  async function desativarJogo(jogo) {
    const confirmar = window.confirm(
      `Tem certeza que deseja desativar o jogo "${jogo.nome}"?`
    )

    if (!confirmar) return

    setErroAcao(null)

    try {
      await jogoApi.excluir(jogo.id)
      setAtualizacao((anterior) => anterior + 1)
    } catch (erro) {
      setErroAcao(obterMensagemErro(erro))
    }
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Administração de jogos</h1>

        <Link to="/admin/jogos/novo" className="btn btn-primary">
          Novo jogo
        </Link>
      </div>

      <form onSubmit={pesquisar} className="mb-4">
        <div className="input-group">
          <input
            type="search"
            className="form-control"
            placeholder="Buscar jogo pelo nome..."
            value={busca}
            onChange={(evento) => setBusca(evento.target.value)}
          />

          <button type="submit" className="btn btn-primary">
            Buscar
          </button>
        </div>
      </form>

      {erroAcao && (
        <div className="alert alert-danger" role="alert">
          {erroAcao}
        </div>
      )}

      {carregando && (
        <Carregando mensagem="Carregando jogos..." />
      )}

      {!carregando && erro && (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={() => setAtualizacao((anterior) => anterior + 1)}
        />
      )}

      {!carregando && !erro && jogos.length === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado." />
      )}

      {!carregando && !erro && jogos.length > 0 && (
        <>
          <div className="table-responsive">
            <table className="table table-striped table-hover align-middle">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Nome</th>
                  <th>Ano</th>
                  <th>Plataforma</th>
                  <th>Condição</th>
                  <th>Gêneros</th>
                  <th>Preço</th>
                  <th>Estoque</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </thead>

              <tbody>
                {jogos.map((jogo) => (
                  <tr key={jogo.id}>
                    <td>{jogo.id}</td>
                    <td>{jogo.nome}</td>
                    <td>{jogo.anoLancamento}</td>
                    <td>{jogo.nomePlataforma}</td>
                    <td>{jogo.descricaoCondicao}</td>
                    <td>{jogo.generos?.join(', ')}</td>
                    <td>{formatarPreco(jogo.preco)}</td>
                    <td>{jogo.estoque}</td>
                    <td>
                      {jogo.ativo ? (
                        <span className="badge bg-success">
                          Ativo
                        </span>
                      ) : (
                        <span className="badge bg-secondary">
                          Inativo
                        </span>
                      )}
                    </td>
                    <td>
                      <div className="d-flex gap-2">
                        <Link
                          to={`/admin/jogos/${jogo.id}/editar`}
                          className="btn btn-outline-primary btn-sm"
                        >
                          Editar
                        </Link>

                        {jogo.ativo && (
                          <button
                            type="button"
                            className="btn btn-outline-danger btn-sm"
                            onClick={() => desativarJogo(jogo)}
                          >
                            Desativar
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <Paginacao
            pagina={pagina}
            totalPaginas={totalPaginas}
            aoMudar={setPagina}
          />
        </>
      )}
    </div>
  )
}

export default Jogos
