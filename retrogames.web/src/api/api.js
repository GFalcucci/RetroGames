
import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  withCredentials: true
})

api.interceptors.response.use(
  resposta => resposta,
  erro => {
    const url = erro.config?.url ?? ''

    const erroEsperado =
      url.includes('/Autenticacao/eu') ||
      url.includes('/Autenticacao/login')

    if (erro.response?.status === 401 && !erroEsperado) {
      window.dispatchEvent(new Event('sessao:expirada'))
    }

    return Promise.reject(erro)
  }
)

export default api
