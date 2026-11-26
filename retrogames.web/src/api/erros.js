
export function obterMensagemErro(erro) {
  return erro.response?.data?.errorMessage
    ?? 'Ocorreu um erro inesperado.'
}

export function obterErrosDosCampos(erro) {
  const dados = erro.response?.data

  if (dados?.responseCode !== 3 || !dados.data) {
    return {}
  }

  return dados.data
}
