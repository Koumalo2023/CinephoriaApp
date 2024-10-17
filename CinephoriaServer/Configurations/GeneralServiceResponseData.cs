namespace CinephoriaServer.Configurations
{
    public class GeneralServiceResponseData<T>
    {
        public bool IsSucceed { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}
