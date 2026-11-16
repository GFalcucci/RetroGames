
using System.Net;

namespace RetroGames.Services;

public sealed record BusinessError(
    int Code,
    string Mensagem,
    HttpStatusCode Status);

public static class BusinessErrors
{
    public static readonly BusinessError PlataformaNaoEncontrada =
        new(7, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static readonly BusinessError PlataformaNomeDuplicado =
        new(8, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    public static readonly BusinessError PlataformaPossuiJogos =
        new(9, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    public static readonly BusinessError JogoNaoEncontrado =
        new(10, "Jogo não encontrado.", HttpStatusCode.NotFound);

    public static readonly BusinessError CondicaoNaoEncontrada =
        new(12, "A condição informada não existe.", HttpStatusCode.NotFound);

    public static readonly BusinessError GeneroNaoEncontrado =
        new(13, "Um ou mais gêneros informados não existem.", HttpStatusCode.NotFound);
}
