export function Carregando({ mensagem = 'Carregando...' }) {
  return (
    <p className="text-muted">{mensagem}</p>
  )
}

export function MensagemErro({
  mensagem,
  aoTentarNovamente
}) {
  return (
    <div className="alert alert-danger" role="alert">
      <p className="mb-2">{mensagem}</p>

      {aoTentarNovamente && (
        <button
          type="button"
          className="btn btn-outline-danger btn-sm"
          onClick={aoTentarNovamente}
        >
          Tentar novamente
        </button>
      )}
    </div>
  )
}

export function EstadoVazio({
  titulo = 'Nenhum resultado encontrado.',
  children
}) {
  return (
    <div className="text-center text-muted py-4">
      <h2 className="h5">{titulo}</h2>
      {children}
    </div>
  )
}