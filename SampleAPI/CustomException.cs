namespace SampleAPI
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {
            
        }
        public new string Message { get; set; }
    }
}
