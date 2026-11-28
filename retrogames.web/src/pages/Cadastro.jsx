import { useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'

import useAutenticacao from '../hooks/useAutenticacao'
import { obterMensagemErro, obterErrosDosCampos } from '../api/erros'

function Cadastro() {
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [errosCampos, setErrosCampos] = useState({})
  const [carregando, setCarregando] = useState(false)

  const [searchParams] = useSearchParams()
  const navigate = useNavigate()

  const { cadastrar } = useAutenticacao()

  const voltarPara = searchParams.get('voltarPara')

  const destino =
    voltarPara?.startsWith('/') && !voltarPara.startsWith('//')
      ? voltarPara
      : '/'

  async function tratarEnvio(evento) {
    evento.preventDefault()

    try {
      setCarregando(true)
      setErro('')
      setErrosCampos({})

      await cadastrar({ nome, email, senha })

      // Após o cadastro a sessão já começou.
      navigate(destino, { replace: true })
    } catch (erro) {
      const campos = obterErrosDosCampos(erro)

      if (Object.keys(campos).length > 0) {
        setErrosCampos(campos)
      } else if (erro.response?.data?.responseCode === 106) {
        // E-mail já cadastrado: o erro aparece embaixo do campo.
        setErrosCampos({ email: [obterMensagemErro(erro)] })
      } else {
        setErro(obterMensagemErro(erro))
      }
    } finally {
      setCarregando(false)
    }
  }

  function classeCampo(campo) {
    return `form-control ${errosCampos[campo] ? 'is-invalid' : ''}`
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-md-6 col-lg-4">
        <div className="card shadow-sm">
          <div className="card-body p-4">
            <h1 className="h3 text-center mb-4">Criar conta</h1>

            {erro && (
              <div className="alert alert-danger" role="alert">
                {erro}
              </div>
            )}

            <form onSubmit={tratarEnvio} noValidate>
              <div className="mb-3">
                <label htmlFor="nome" className="form-label">
                  Nome
                </label>

                <input
                  id="nome"
                  type="text"
                  className={classeCampo('nome')}
                  value={nome}
                  onChange={(evento) => setNome(evento.target.value)}
                  maxLength={120}
                  required
                  autoComplete="name"
                />

                {errosCampos.nome && (
                  <div className="invalid-feedback">
                    {errosCampos.nome[0]}
                  </div>
                )}
              </div>

              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  E-mail
                </label>

                <input
                  id="email"
                  type="email"
                  className={classeCampo('email')}
                  value={email}
                  onChange={(evento) => setEmail(evento.target.value)}
                  maxLength={254}
                  required
                  autoComplete="username"
                />

                {errosCampos.email && (
                  <div className="invalid-feedback">
                    {errosCampos.email[0]}
                  </div>
                )}
              </div>

              <div className="mb-3">
                <label htmlFor="senha" className="form-label">
                  Senha
                </label>

                <input
                  id="senha"
                  type="password"
                  className={classeCampo('senha')}
                  value={senha}
                  onChange={(evento) => setSenha(evento.target.value)}
                  aria-describedby="ajuda-senha"
                  required
                  autoComplete="new-password"
                />

                {errosCampos.senha && (
                  <div className="invalid-feedback">
                    {errosCampos.senha[0]}
                  </div>
                )}

                <div id="ajuda-senha" className="form-text">
                  Use pelo menos 8 caracteres.
                </div>
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={carregando}
              >
                {carregando ? 'Criando conta...' : 'Criar conta'}
              </button>
            </form>

            <p className="text-center mt-3 mb-0">
              Já tem conta?{' '}
              <Link
                to={
                  voltarPara
                    ? `/login?voltarPara=${encodeURIComponent(voltarPara)}`
                    : '/login'
                }
              >
                Entrar
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Cadastro
