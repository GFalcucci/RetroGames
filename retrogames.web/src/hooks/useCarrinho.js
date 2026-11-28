import { useContext } from 'react'
import carrinhoContext from '../contexts/carrinhoContext'

function useCarrinho() {
  const contexto = useContext(carrinhoContext)

  if (!contexto) {
    throw new Error(
      'useCarrinho deve ser utilizado dentro de CarrinhoProvider.'
    )
  }

  return contexto
}

export default useCarrinho
