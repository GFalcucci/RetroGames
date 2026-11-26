
import { useCallback, useEffect, useState } from 'react'
import autenticacaoContext from './autenticacaoContext'
import { autenticacaoApi } from '../api/servicos'

function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  const entrar = useCallback(async (dados) => {
    const usuarioAutenticado = await autenticacaoApi.entrar(dados)
    setUsuario(usuarioAutenticado)
    return usuarioAutenticado
  }, [])

  const sair = useCallback(async () => {
    try {
      await autenticacaoApi.sair()
    } finally {
      setUsuario(null)
    }
  }, [])

  useEffect(() => {
    let cancelado = false

    async function carregarUsuario() {
      try {
        const usuarioAtual = await autenticacaoApi.usuarioAtual()

        if (!cancelado) {
          setUsuario(usuarioAtual)
        }
      } catch {
        if (!cancelado) {
          setUsuario(null)
        }
      } finally {
        if (!cancelado) {
          setCarregando(false)
        }
      }
    }

    function expirarSessao() {
      setUsuario(null)
      setCarregando(false)
    }

    window.addEventListener('sessao:expirada', expirarSessao)

    carregarUsuario()

    return () => {
      cancelado = true
      window.removeEventListener('sessao:expirada', expirarSessao)
    }
  }, [])

  const valor = {
    usuario,
    carregando,
    estaAutenticado: Boolean(usuario),
    ehAdministrador: usuario?.idPerfil === 1,
    entrar,
    sair
  }

  return (
    <autenticacaoContext.Provider value={valor}>
      {children}
    </autenticacaoContext.Provider>
  )
}

export default AutenticacaoProvider
