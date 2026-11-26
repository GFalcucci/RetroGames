
import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'

import {
  jogoApi,
  plataformaApi,
  generoApi,
  condicaoJogoApi
} from '../../api/servicos'

import {
  obterMensagemErro,
  obterErrosDosCampos
} from '../../api/erros'

import { Carregando } from '../../components/MensagensEstado'

const formularioInicial = {
  nome: '',
  descricao: '',
  anoLancamento: '',
  preco: '',
  estoque: '',
  idPlataforma: '',
  idCondicaoJogo: '',
  idsGeneros: []
}

function FormularioJogo() {
  const { id } = useParams()
  const navegar = useNavigate()
  const estaEditando = Boolean(id)

  const [formulario, setFormulario] = useState(formularioInicial)
  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [condicoes, setCondicoes] = useState([])

  const [carregando, setCarregando] = useState(true)
  const [salvando, setSalvando] = useState(false)
  const [erro, setErro] = useState(null)
  const [errosCampos, setErrosCampos] = useState({})

  useEffect(() => {
    async function carregarDados() {
      setCarregando(true)
      setErro(null)

      try {
        const [listaPlataformas, listaGeneros, listaCondicoes] =
          await Promise.all([
            plataformaApi.listar(),
            generoApi.listar(),
            condicaoJogoApi.listar()
          ])

        setPlataformas(listaPlataformas ?? [])
        setGeneros(listaGeneros ?? [])
        setCondicoes(listaCondicoes ?? [])

        if (estaEditando) {
          const jogo = await jogoApi.buscarPorId(id)

          setFormulario({
            nome: jogo.nome ?? '',
            descricao: jogo.descricao ?? '',
            anoLancamento: String(jogo.anoLancamento ?? ''),
            preco: String(jogo.preco ?? ''),
            estoque: String(jogo.estoque ?? ''),
            idPlataforma: String(jogo.idPlataforma ?? ''),
            idCondicaoJogo: String(jogo.idCondicaoJogo ?? ''),
            idsGeneros: jogo.idsGeneros ?? []
          })
        }
      } catch (erro) {
        setErro(obterMensagemErro(erro))
      } finally {
        setCarregando(false)
      }
    }

    carregarDados()
  }, [id, estaEditando])

  function atualizarCampo(evento) {
    const { name, value } = evento.target

    setFormulario((anterior) => ({
      ...anterior,
      [name]: value
    }))

    setErrosCampos((anteriores) => ({
      ...anteriores,
      [name]: []
    }))
  }

  function atualizarGenero(idGenero) {
    setFormulario((anterior) => {
      const jaSelecionado = anterior.idsGeneros.includes(idGenero)

      return {
        ...anterior,
        idsGeneros: jaSelecionado
          ? anterior.idsGeneros.filter((id) => id !== idGenero)
          : [...anterior.idsGeneros, idGenero]
      }
    })

    setErrosCampos((anteriores) => ({
      ...anteriores,
      idsGeneros: []
    }))
  }

  async function salvarJogo(evento) {
    evento.preventDefault()

    setSalvando(true)
    setErro(null)
    setErrosCampos({})

    const dados = {
      nome: formulario.nome.trim(),
      descricao: formulario.descricao.trim(),
      anoLancamento: Number(formulario.anoLancamento),
      preco: Number(formulario.preco),
      estoque: Number(formulario.estoque),
      idPlataforma: Number(formulario.idPlataforma),
      idCondicaoJogo: Number(formulario.idCondicaoJogo),
      idsGeneros: formulario.idsGeneros
    }

    try {
      if (estaEditando) {
        await jogoApi.alterar(id, dados)
      } else {
        await jogoApi.inserir(dados)
      }

      navegar('/admin/jogos')
    } catch (erro) {
      setErro(obterMensagemErro(erro))
      setErrosCampos(obterErrosDosCampos(erro))
    } finally {
      setSalvando(false)
    }
  }

  function campoInvalido(nomeCampo) {
    const mensagens = errosCampos[nomeCampo]

    if (Array.isArray(mensagens)) {
      return mensagens.length > 0
    }

    return Boolean(mensagens)
  }

  function exibirErroCampo(nomeCampo) {
    const mensagens = errosCampos[nomeCampo]

    if (!campoInvalido(nomeCampo)) return null

    return (
      <div className="invalid-feedback">
        {Array.isArray(mensagens)
          ? mensagens.join(' ')
          : mensagens}
      </div>
    )
  }

  if (carregando) {
    return <Carregando mensagem="Carregando formulário..." />
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>
          {estaEditando ? 'Editar jogo' : 'Novo jogo'}
        </h1>

        <Link to="/admin/jogos" className="btn btn-outline-secondary">
          Voltar
        </Link>
      </div>

      {erro && (
        <div className="alert alert-danger" role="alert">
          {erro}
        </div>
      )}

      <form onSubmit={salvarJogo} noValidate>
        <div className="card">
          <div className="card-body">
            <div className="mb-3">
              <label htmlFor="nome" className="form-label">
                Nome
              </label>

              <input
                id="nome"
                name="nome"
                type="text"
                className={`form-control ${campoInvalido('nome') ? 'is-invalid' : ''}`}
                value={formulario.nome}
                onChange={atualizarCampo}
                required
              />

              {exibirErroCampo('nome')}
            </div>

            <div className="mb-3">
              <label htmlFor="descricao" className="form-label">
                Descrição
              </label>

              <textarea
                id="descricao"
                name="descricao"
                className={`form-control ${campoInvalido('descricao') ? 'is-invalid' : ''}`}
                rows="3"
                value={formulario.descricao}
                onChange={atualizarCampo}
              />

              {exibirErroCampo('descricao')}
            </div>

            <div className="row">
              <div className="col-md-4 mb-3">
                <label htmlFor="anoLancamento" className="form-label">
                  Ano de lançamento
                </label>

                <input
                  id="anoLancamento"
                  name="anoLancamento"
                  type="number"
                  min="1970"
                  max="2100"
                  className={`form-control ${campoInvalido('anoLancamento') ? 'is-invalid' : ''}`}
                  value={formulario.anoLancamento}
                  onChange={atualizarCampo}
                  required
                />

                {exibirErroCampo('anoLancamento')}
              </div>

              <div className="col-md-4 mb-3">
                <label htmlFor="preco" className="form-label">
                  Preço
                </label>

                <input
                  id="preco"
                  name="preco"
                  type="number"
                  min="0.01"
                  step="0.01"
                  className={`form-control ${campoInvalido('preco') ? 'is-invalid' : ''}`}
                  value={formulario.preco}
                  onChange={atualizarCampo}
                  required
                />

                {exibirErroCampo('preco')}
              </div>

              <div className="col-md-4 mb-3">
                <label htmlFor="estoque" className="form-label">
                  Estoque
                </label>

                <input
                  id="estoque"
                  name="estoque"
                  type="number"
                  min="0"
                  className={`form-control ${campoInvalido('estoque') ? 'is-invalid' : ''}`}
                  value={formulario.estoque}
                  onChange={atualizarCampo}
                  required
                />

                {exibirErroCampo('estoque')}
              </div>
            </div>

            <div className="mb-3">
              <label htmlFor="idPlataforma" className="form-label">
                Plataforma
              </label>

              <select
                id="idPlataforma"
                name="idPlataforma"
                className={`form-select ${campoInvalido('idPlataforma') ? 'is-invalid' : ''}`}
                value={formulario.idPlataforma}
                onChange={atualizarCampo}
                required
              >
                <option value="">Selecione uma plataforma</option>

                {plataformas.map((plataforma) => (
                  <option key={plataforma.id} value={plataforma.id}>
                    {plataforma.nome}
                  </option>
                ))}
              </select>

              {exibirErroCampo('idPlataforma')}
            </div>

            <div className="mb-3">
              <label htmlFor="idCondicaoJogo" className="form-label">
                Condição do jogo
              </label>

              <select
                id="idCondicaoJogo"
                name="idCondicaoJogo"
                className={`form-select ${campoInvalido('idCondicaoJogo') ? 'is-invalid' : ''}`}
                value={formulario.idCondicaoJogo}
                onChange={atualizarCampo}
                required
              >
                <option value="">Selecione uma condição</option>

                {condicoes.map((condicao) => (
                  <option key={condicao.id} value={condicao.id}>
                    {condicao.descricao}
                  </option>
                ))}
              </select>

              {exibirErroCampo('idCondicaoJogo')}
            </div>

            <div className="mb-3">
              <label className="form-label">
                Gêneros
              </label>

              {generos.map((genero) => (
                <div className="form-check" key={genero.id}>
                  <input
                    id={`genero-${genero.id}`}
                    type="checkbox"
                    className="form-check-input"
                    checked={formulario.idsGeneros.includes(genero.id)}
                    onChange={() => atualizarGenero(genero.id)}
                  />

                  <label
                    htmlFor={`genero-${genero.id}`}
                    className="form-check-label"
                  >
                    {genero.nome}
                  </label>
                </div>
              ))}

              {exibirErroCampo('idsGeneros')}
            </div>

            <div className="d-flex gap-2">
              <button
                type="submit"
                className="btn btn-primary"
                disabled={salvando}
              >
                {salvando ? 'Salvando...' : 'Salvar'}
              </button>

              <Link
                to="/admin/jogos"
                className="btn btn-outline-secondary"
              >
                Cancelar
              </Link>
            </div>
          </div>
        </div>
      </form>
    </div>
  )
}

export default FormularioJogo
