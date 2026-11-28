
import { useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'

import useAutenticacao from '../hooks/useAutenticacao'
import { obterMensagemErro } from '../api/erros'

function Login() {
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [carregando, setCarregando] = useState(false)

  const [searchParams] = useSearchParams()
  const navigate = useNavigate()

  const { entrar } = useAutenticacao()

  const voltarPara = searchParams.get('voltarPara')

  async function tratarEnvio(evento) {
    evento.preventDefault()

    try {
      setCarregando(true)
      setErro('')

      await entrar({ email, senha })

      const destino =
        voltarPara?.startsWith('/') && !voltarPara.startsWith('//')
          ? voltarPara
          : '/'

      navigate(destino, { replace: true })
    } catch (erro) {
      setErro(obterMensagemErro(erro))
    } finally {
      setCarregando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-md-6 col-lg-4">
        <div className="card shadow-sm">
          <div className="card-body p-4">
            <h1 className="h3 text-center mb-4">Entrar</h1>

            {erro && (
              <div className="alert alert-danger" role="alert">
                {erro}
              </div>
            )}

            <form onSubmit={tratarEnvio}>
              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  E-mail
                </label>

                <input
                  id="email"
                  type="email"
                  className="form-control"
                  value={email}
                  onChange={(evento) => setEmail(evento.target.value)}
                  required
                  autoComplete="username"
                />
              </div>

              <div className="mb-3">
                <label htmlFor="senha" className="form-label">
                  Senha
                </label>

                <input
                  id="senha"
                  type="password"
                  className="form-control"
                  value={senha}
                  onChange={(evento) => setSenha(evento.target.value)}
                  required
                  autoComplete="current-password"
                />
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={carregando}
              >
                {carregando ? 'Entrando...' : 'Entrar'}
              </button>
            </form>

            <p className="text-center mt-3 mb-0">
              Ainda não tem conta?{' '}
              <Link
                to={
                  voltarPara
                    ? `/cadastro?voltarPara=${encodeURIComponent(voltarPara)}`
                    : '/cadastro'
                }
              >
                Cadastre-se
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Login
