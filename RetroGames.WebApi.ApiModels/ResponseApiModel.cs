namespace RetroGames.WebApi.ApiModels;

public class ResponseApiModel
{
    public int ResponseCode { get; set; }

    public string? ErrorMessage { get; set; }
}

public class ResponseApiModel<T> : ResponseApiModel
{
    public T? Data { get; set; }
}