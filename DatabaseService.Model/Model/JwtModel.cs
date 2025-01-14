namespace DatabaseService.Model.Model
{
    public class JwtModel
    {
        public string Issuer
        {
            get; set;
        }

        public string Audience
        {
            get; set;
        }

        public string PrivateKey
        {
            get; set;
        }

    }
}
