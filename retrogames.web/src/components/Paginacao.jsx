function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) {
    return null
  }

  return (
    <nav aria-label="Paginação do catálogo">
      <ul className="pagination justify-content-center mt-4">
        <li className={`page-item ${pagina <= 1 ? 'disabled' : ''}`}>
          <button
            type="button"
            className="page-link"
            disabled={pagina <= 1}
            onClick={() => aoMudar(pagina - 1)}
          >
            Anterior
          </button>
        </li>

        {Array.from({ length: totalPaginas }, (_, indice) => {
          const numeroPagina = indice + 1

          return (
            <li
              key={numeroPagina}
              className={`page-item ${
                pagina === numeroPagina ? 'active' : ''
              }`}
            >
              <button
                type="button"
                className="page-link"
                onClick={() => aoMudar(numeroPagina)}
              >
                {numeroPagina}
              </button>
            </li>
          )
        })}

        <li
          className={`page-item ${
            pagina >= totalPaginas ? 'disabled' : ''
          }`}
        >
          <button
            type="button"
            className="page-link"
            disabled={pagina >= totalPaginas}
            onClick={() => aoMudar(pagina + 1)}
          >
            Próxima
          </button>
        </li>
      </ul>
    </nav>
  )
}

export default Paginacao