
import { useEffect, useState } from 'react'
import { plataformaApi } from '../../api/servicos'
import {
  obterMensagemErro,
  obterErrosDosCampos
} from '../../api/erros'
import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'

const formularioInicial = {
  nome: '',
  fabricante: '',
  anoLancamento: ''
}

function Plataformas() {
  const [plataformas, setPlataformas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)

  const [formulario, setFormulario] = useState(formularioInicial)
  const [idEmEdicao, setIdEmEdicao] = useState(null)
  const [formularioVisivel, setFormularioVisivel] = useState(false)
  const [salvando, setSalvando] = useState(false)

  const [erroFormulario, setErroFormulario] = useState(null)
  const [errosCampos, setErrosCampos] = useState({})
  const [erroExclusao, setErroExclusao] = useState(null)

  async function carregarPlataformas() {
    setCarregando(true)
    setErro(null)

    try {
      const dados = await plataformaApi.listar()
      setPlataformas(dados ?? [])
    } catch (erro) {
      setErro(obterMensagemErro(erro))
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregarPlataformas()
  }, [])

  function limparFormulario() {
    setFormulario(formularioInicial)
    setIdEmEdicao(null)
    setFormularioVisivel(false)
    setErroFormulario(null)
    setErrosCampos({})
  }

  function iniciarNovoCadastro() {
    setFormulario(formularioInicial)
    setIdEmEdicao(null)
    setErroFormulario(null)
    setErrosCampos({})
    setFormularioVisivel(true)
  }

  function iniciarEdicao(plataforma) {
    setFormulario({
      nome: plataforma.nome,
      fabricante: plataforma.fabricante,
      anoLancamento: String(plataforma.anoLancamento)
    })

    setIdEmEdicao(plataforma.id)
    setErroFormulario(null)
    setErrosCampos({})
    setFormularioVisivel(true)
  }

  function atualizarCampo(evento) {
    const { name, value } = evento.target

    setFormulario((anterior) => ({
      ...anterior,
      [name]: value
    }))

    setErrosCampos((anterior) => ({
      ...anterior,
      [name]: undefined
    }))
  }

  async function salvarPlataforma(evento) {
    evento.preventDefault()

    setSalvando(true)
    setErroFormulario(null)
    setErrosCampos({})

    const dados = {
      nome: formulario.nome,
      fabricante: formulario.fabricante,
      anoLancamento: Number(formulario.anoLancamento)
    }

    try {
      if (idEmEdicao !== null) {
        await plataformaApi.alterar(idEmEdicao, dados)
      } else {
        await plataformaApi.inserir(dados)
      }

      limparFormulario()
      await carregarPlataformas()
    } catch (erro) {
      setErroFormulario(obterMensagemErro(erro))
      setErrosCampos(obterErrosDosCampos(erro))
    } finally {
      setSalvando(false)
    }
  }

  async function excluirPlataforma(plataforma) {
    const confirmar = window.confirm(
      `Tem certeza que deseja excluir a plataforma "${plataforma.nome}"?`
    )

    if (!confirmar) return

    setErroExclusao(null)

    try {
      await plataformaApi.excluir(plataforma.id)
      await carregarPlataformas()
    } catch (erro) {
      setErroExclusao(obterMensagemErro(erro))
    }
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Administração de plataformas</h1>

        <button
          type="button"
          className="btn btn-primary"
          onClick={iniciarNovoCadastro}
        >
          Nova plataforma
        </button>
      </div>

      {erroExclusao && (
        <div className="alert alert-danger" role="alert">
          {erroExclusao}
        </div>
      )}

      {formularioVisivel && (
        <div className="card mb-4">
          <div className="card-body">
            <h2 className="h5 mb-3">
              {idEmEdicao !== null
                ? 'Editar plataforma'
                : 'Nova plataforma'}
            </h2>

            {erroFormulario && (
              <div className="alert alert-danger" role="alert">
                {erroFormulario}
              </div>
            )}

            <form onSubmit={salvarPlataforma}>
              <div className="mb-3">
                <label htmlFor="nome" className="form-label">
                  Nome
                </label>

                <input
                  type="text"
                  id="nome"
                  name="nome"
                  className={`form-control ${errosCampos.nome ? 'is-invalid' : ''}`}
                  value={formulario.nome}
                  onChange={atualizarCampo}
                />

                {errosCampos.nome && (
                  <div className="invalid-feedback">
                    {errosCampos.nome.join(' ')}
                  </div>
                )}
              </div>

              <div className="mb-3">
                <label htmlFor="fabricante" className="form-label">
                  Fabricante
                </label>

                <input
                  type="text"
                  id="fabricante"
                  name="fabricante"
                  className={`form-control ${errosCampos.fabricante ? 'is-invalid' : ''}`}
                  value={formulario.fabricante}
                  onChange={atualizarCampo}
                />

                {errosCampos.fabricante && (
                  <div className="invalid-feedback">
                    {errosCampos.fabricante.join(' ')}
                  </div>
                )}
              </div>

              <div className="mb-3">
                <label htmlFor="anoLancamento" className="form-label">
                  Ano de lançamento
                </label>

                <input
                  type="number"
                  id="anoLancamento"
                  name="anoLancamento"
                  className={`form-control ${errosCampos.anoLancamento ? 'is-invalid' : ''}`}
                  value={formulario.anoLancamento}
                  onChange={atualizarCampo}
                />

                {errosCampos.anoLancamento && (
                  <div className="invalid-feedback">
                    {errosCampos.anoLancamento.join(' ')}
                  </div>
                )}
              </div>

              <button
                type="submit"
                className="btn btn-success me-2"
                disabled={salvando}
              >
                {salvando ? 'Salvando...' : 'Salvar'}
              </button>

              <button
                type="button"
                className="btn btn-secondary"
                onClick={limparFormulario}
                disabled={salvando}
              >
                Cancelar
              </button>
            </form>
          </div>
        </div>
      )}

      {carregando && (
        <Carregando mensagem="Carregando plataformas..." />
      )}

      {!carregando && erro && (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={carregarPlataformas}
        />
      )}

      {!carregando && !erro && plataformas.length === 0 && (
        <EstadoVazio titulo="Nenhuma plataforma cadastrada." />
      )}

      {!carregando && !erro && plataformas.length > 0 && (
        <div className="table-responsive">
          <table className="table table-striped table-hover">
            <thead>
              <tr>
                <th>ID</th>
                <th>Nome</th>
                <th>Fabricante</th>
                <th>Ano de lançamento</th>
                <th>Ações</th>
              </tr>
            </thead>

            <tbody>
              {[...plataformas]
                .sort((a, b) => a.id - b.id)
                .map((plataforma) => (
                  <tr key={plataforma.id}>
                    <td>{plataforma.id}</td>
                    <td>{plataforma.nome}</td>
                    <td>{plataforma.fabricante}</td>
                    <td>{plataforma.anoLancamento}</td>
                    <td>
                      <button
                        type="button"
                        className="btn btn-outline-primary btn-sm"
                        onClick={() => iniciarEdicao(plataforma)}
                      >
                        Editar
                      </button>

                      <button
                        type="button"
                        className="btn btn-outline-danger btn-sm ms-2"
                        onClick={() => excluirPlataforma(plataforma)}
                      >
                        Excluir
                      </button>
                    </td>
                  </tr>
                ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

export default Plataformas
