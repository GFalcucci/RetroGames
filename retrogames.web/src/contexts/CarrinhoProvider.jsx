import { useCallback, useEffect, useMemo, useState } from 'react'
import carrinhoContext from './carrinhoContext'
import {
  lerCarrinho,
  salvarCarrinho,
  limitarQuantidade
} from '../lib/carrinho'

function CarrinhoProvider({ children }) {
  const [itens, setItens] = useState(lerCarrinho)

  useEffect(() => {
    salvarCarrinho(itens)
  }, [itens])

  const adicionar = useCallback((jogo, quantidade = 1) => {
    setItens(atuais => {
      const existente = atuais.find(item => item.idJogo === jogo.id)

      if (existente) {
        return atuais.map(item =>
          item.idJogo === jogo.id
            ? {
                ...item,
                preco: jogo.preco,
                estoque: jogo.estoque,
                quantidade: limitarQuantidade(
                  item.quantidade + quantidade,
                  jogo.estoque
                )
              }
            : item
        )
      }

      return [
        ...atuais,
        {
          idJogo: jogo.id,
          nome: jogo.nome,
          nomePlataforma: jogo.nomePlataforma,
          preco: jogo.preco,
          estoque: jogo.estoque,
          quantidade: limitarQuantidade(quantidade, jogo.estoque)
        }
      ]
    })
  }, [])

  const alterarQuantidade = useCallback((idJogo, quantidade) => {
    setItens(atuais =>
      atuais.map(item =>
        item.idJogo === idJogo
          ? {
              ...item,
              quantidade: limitarQuantidade(quantidade, item.estoque)
            }
          : item
      )
    )
  }, [])

  const remover = useCallback(idJogo => {
    setItens(atuais => atuais.filter(item => item.idJogo !== idJogo))
  }, [])

  const limpar = useCallback(() => {
    setItens([])
  }, [])

  const totalItens = useMemo(
    () => itens.reduce((soma, item) => soma + item.quantidade, 0),
    [itens]
  )

  const total = useMemo(
    () => itens.reduce(
      (soma, item) => soma + item.preco * item.quantidade,
      0
    ),
    [itens]
  )

  const valor = {
    itens,
    totalItens,
    total,
    adicionar,
    alterarQuantidade,
    remover,
    limpar
  }

  return (
    <carrinhoContext.Provider value={valor}>
      {children}
    </carrinhoContext.Provider>
  )
}

export default CarrinhoProvider
