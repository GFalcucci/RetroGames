namespace RetroGames.Services.Interfaces;

public interface IUsuarioAtual
{
    int? Id { get; }
    int? IdPerfil { get; }
    bool EhAdministrador { get; }
}