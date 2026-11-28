function IconePessoa({ tamanho = 24 }) {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 24 24"
      width={tamanho}
      height={tamanho}
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
      focusable="false"
    >
      {/* Cabeça */}
      <circle cx="12" cy="8" r="4" />

      {/* Corpo */}
      <path d="M5 21v-2a7 7 0 0 1 14 0v2" />
    </svg>
  )
}

export default IconePessoa
