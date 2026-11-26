
import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'

import {
  plataformaApi,
  generoApi,
  jogoApi
} from '../api/servicos'

import CardJogo from '../components/CardJogo'
import Paginacao from '../components/Paginacao'

import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../components/MensagensEstado'

import { obterMensagemErro } from '../api/erros'

function Catalogo() {
  const [parametrosUrl, setParametrosUrl] = useSearchParams()

  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])

  const [jogos, setJogos] = useState([])
  const [pagina, setPagina] = useState(1)
  const [totalPaginas, setTotalPaginas] = useState(1)

  const [buscaDigitada, setBuscaDigitada] = useState(
    parametrosUrl.get('busca') ?? ''
  )

  const [carregandoFiltros, setCarregandoFiltros] = useState(true)
  const [carregandoJogos, setCarregandoJogos] = useState(true)

  const [erroFiltros, setErroFiltros] = useState('')
  const [erroJogos, setErroJogos] = useState('')

  const [tentativaFiltros, setTentativaFiltros] = useState(0)
  const [tentativaJogos, setTentativaJogos] = useState(0)

  const busca = parametrosUrl.get('busca') ?? ''
  const idPlataforma = parametrosUrl.get('idPlataforma') ?? ''
  const idGenero = parametrosUrl.get('idGenero') ?? ''
  const paginaUrl = Number(parametrosUrl.get('pagina') || 1)

  useEffect(() => {
    async function carregarFiltros() {
      try {
        setCarregandoFiltros(true)
        setErroFiltros('')

        const [listaPlataformas, listaGeneros] = await Promise.all([
          plataformaApi.listar(),
          generoApi.listar()
        ])

        setPlataformas(listaPlataformas)
        setGeneros(listaGeneros)
      } catch (erro) {
        setErroFiltros(obterMensagemErro(erro))
      } finally {
        setCarregandoFiltros(false)
      }
    }

    carregarFiltros()
  }, [tentativaFiltros])

  useEffect(() => {
    setBuscaDigitada(busca)
  }, [busca])

  useEffect(() => {
    let cancelado = false

    async function carregarJogos() {
      try {
        setCarregandoJogos(true)
        setErroJogos('')

        const dados = await jogoApi.listar({
          busca: busca || undefined,
          idPlataforma: idPlataforma || undefined,
          idGenero: idGenero || undefined,
          pagina: paginaUrl,
          tamanhoPagina: 10
        })

        if (!cancelado) {
          setJogos(dados.itens)
          setPagina(dados.pagina)
          setTotalPaginas(dados.totalPaginas)
        }
      } catch (erro) {
        if (!cancelado) {
          setErroJogos(obterMensagemErro(erro))
        }
      } finally {
        if (!cancelado) {
          setCarregandoJogos(false)
        }
      }
    }

    carregarJogos()

    return () => {
      cancelado = true
    }
  }, [
    busca,
    idPlataforma,
    idGenero,
    paginaUrl,
    tentativaJogos
  ])

  function atualizarFiltros(novosFiltros) {
    const novosParametros = new URLSearchParams(parametrosUrl)

    Object.entries(novosFiltros).forEach(([chave, valor]) => {
      if (valor) {
        novosParametros.set(chave, valor)
      } else {
        novosParametros.delete(chave)
      }
    })

    novosParametros.set('pagina', '1')

    setParametrosUrl(novosParametros)
  }

  function pesquisar(evento) {
    evento.preventDefault()

    atualizarFiltros({
      busca: buscaDigitada.trim()
    })
  }

  function mudarPagina(novaPagina) {
    const novosParametros = new URLSearchParams(parametrosUrl)

    novosParametros.set('pagina', String(novaPagina))

    setParametrosUrl(novosParametros)
  }

  if (carregandoFiltros) {
    return <Carregando mensagem="Carregando filtros..." />
  }

  if (erroFiltros) {
    return (
      <MensagemErro
        mensagem={erroFiltros}
        aoTentarNovamente={() => setTentativaFiltros(valor => valor + 1)}
      />
    )
  }

  return (
    <section>
      <h1 className="mb-4">Catálogo de jogos</h1>

      <form
        onSubmit={pesquisar}
        className="row g-3 mb-4 catalogo-filtros"
      >
        <div className="col-md-4">
          <label htmlFor="busca" className="form-label">
            Buscar jogo
          </label>

          <input
            id="busca"
            type="search"
            className="form-control"
            placeholder="Digite o nome do jogo"
            value={buscaDigitada}
            onChange={evento => setBuscaDigitada(evento.target.value)}
          />
        </div>

        <div className="col-md-3">
          <label htmlFor="plataforma" className="form-label">
            Plataforma
          </label>

          <select
            id="plataforma"
            className="form-select"
            value={idPlataforma}
            onChange={evento =>
              atualizarFiltros({ idPlataforma: evento.target.value })
            }
          >
            <option value="">Todas as plataformas</option>

            {plataformas.map(plataforma => (
              <option key={plataforma.id} value={plataforma.id}>
                {plataforma.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-md-3">
          <label htmlFor="genero" className="form-label">
            Gênero
          </label>

          <select
            id="genero"
            className="form-select"
            value={idGenero}
            onChange={evento =>
              atualizarFiltros({ idGenero: evento.target.value })
            }
          >
            <option value="">Todos os gêneros</option>

            {generos.map(genero => (
              <option key={genero.id} value={genero.id}>
                {genero.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-md-2 d-flex align-items-end">
          <button type="submit" className="btn btn-primary w-100">
            Buscar
          </button>
        </div>
      </form>

      {carregandoJogos ? (
        <Carregando mensagem="Carregando jogos..." />
      ) : erroJogos ? (
        <MensagemErro
          mensagem={erroJogos}
          aoTentarNovamente={() => setTentativaJogos(valor => valor + 1)}
        />
      ) : jogos.length === 0 ? (
        <EstadoVazio titulo="Nenhum jogo encontrado." />
      ) : (
        <>
          <div className="row g-4">
            {jogos.map(jogo => (
              <div className="col-md-6 col-lg-4" key={jogo.id}>
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>

          <Paginacao
            pagina={pagina}
            totalPaginas={totalPaginas}
            aoMudar={mudarPagina}
          />
        </>
      )}
    </section>
  )
}

export default Catalogo
