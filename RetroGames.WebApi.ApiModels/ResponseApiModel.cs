namespace RetroGames.WebApi.ApiModels;

public class ResponseApiModel<T>
{
    public int ResponseCode { get; set; }

    public T? Data { get; set; }

    public string? ErrorMessage { get; set; }
}