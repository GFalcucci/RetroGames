export const QUANTIDADE_MINIMA = 1
export const QUANTIDADE_MAXIMA = 10

const CHAVE = 'retrogames.carrinho'

// Mantém a quantidade entre 1 e o menor valor entre 10 e o estoque.
export function limitarQuantidade(quantidade, estoque = QUANTIDADE_MAXIMA) {
  const teto = Math.max(QUANTIDADE_MINIMA, Math.min(QUANTIDADE_MAXIMA, estoque))
  const numero = Number(quantidade) || QUANTIDADE_MINIMA

  return Math.max(QUANTIDADE_MINIMA, Math.min(numero, teto))
}

// O carrinho fica só no navegador. Não é token de sessão (RS01).
// O que está aqui serve apenas para exibir: o preço cobrado vem do banco.
export function lerCarrinho() {
  try {
    const bruto = localStorage.getItem(CHAVE)
    const itens = bruto ? JSON.parse(bruto) : []

    if (!Array.isArray(itens)) {
      return []
    }

    return itens.filter(item =>
      Number.isInteger(item?.idJogo) &&
      Number.isInteger(item?.quantidade) &&
      typeof item?.nome === 'string' &&
      typeof item?.preco === 'number'
    )
  } catch {
    return []
  }
}

export function salvarCarrinho(itens) {
  try {
    localStorage.setItem(CHAVE, JSON.stringify(itens))
  } catch {
    // Sem armazenamento disponível: o carrinho vale só nesta visita.
  }
}
