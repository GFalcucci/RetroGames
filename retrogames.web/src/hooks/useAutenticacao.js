import { useContext } from 'react'
import autenticacaoContext from '../contexts/autenticacaoContext'

function useAutenticacao() {
  const contexto = useContext(autenticacaoContext)

  if (!contexto) {
    throw new Error(
      'useAutenticacao deve ser utilizado dentro de AutenticacaoProvider.'
    )
  }

  return contexto
}

export default useAutenticacao