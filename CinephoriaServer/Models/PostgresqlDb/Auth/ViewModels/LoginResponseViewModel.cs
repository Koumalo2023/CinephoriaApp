namespace CinephoriaServer.Models.PostgresqlDb
{
    public class LoginResponseViewModel
    {
        public bool IsSucceed { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string NewToken { get; set; }
        public UserInfos UserInfo { get; set; }
    }
}
