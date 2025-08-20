namespace Global.Anticipation.Infra.CrossCutting.Results
{
    public interface IResult<T>: IResult
    {
        new T Response { get; set; }
    }

    public interface IResult
    {
        EStatusResult StatusResult { get; set; }
        IList<string> Erros { get; set; }
    }
}