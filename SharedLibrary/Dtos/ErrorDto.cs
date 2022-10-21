
namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        public List<string>? Errors { get; private set; }
        public int IsShow { get; private set; }

        public ErrorDto()
        {
            Errors = new List<string>();
        }
        public ErrorDto(String error, bool isShow)
        {
            Errors.Add(error);
            isShow = true;
        }
        public ErrorDto(List<string> errors, int isShow)
        {
            Errors = errors;
            IsShow = isShow;
        }
    }
}
