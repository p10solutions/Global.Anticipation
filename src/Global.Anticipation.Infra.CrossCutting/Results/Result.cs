namespace Global.Anticipation.Infra.CrossCutting.Results
{
    public record Result: IResult
    {
        public EStatusResult StatusResult { get; set; }
        public IList<string> Erros { get; set; } = new List<string>();

        public bool IsSuccess => StatusResult == EStatusResult.Success;
        public bool IsFailure => !IsSuccess;

        protected Result() { }

        public static class ResultFactory
        {
            public static Result Success() => new() { StatusResult = EStatusResult.Success };

            public static Result NotFound() => new() { StatusResult = EStatusResult.NotFound };

            public static Result BusinessValidationFailure(IList<string> errors) => new() { Erros = errors, StatusResult = EStatusResult.BusinessError };

            public static Result BusinessValidationFailure(string error) => BusinessValidationFailure([error]);

            public static Result InternalError(IList<string> errors) => new() { Erros = errors, StatusResult = EStatusResult.ExceptionError };

            public static Result InternalError(string error) => InternalError([error]);
        }
    }


    public record Result<T> : Result, IResult<T>
    {
        public T Response { get; set; }

        internal Result() { }

        private Result(T response, EStatusResult statusResult)
        {
            Response = response;
            StatusResult = statusResult;
        }

        private Result(IList<string> erros, EStatusResult statusResult)
        {
            StatusResult = statusResult;
            Erros = erros;
        }

        private Result(EStatusResult statusResult)
        {
            StatusResult = statusResult;
        }

        public static class ResultFactory
        {
            public static Result<T> Success(T response) => new(response, EStatusResult.Success);

            public static Result<T> NotFound() => new(EStatusResult.NotFound);

            public static Result<T> BusinessValidationFailure(IList<string> errors) => new(errors, EStatusResult.BusinessError);

            public static Result<T> BusinessValidationFailure(string erro) => BusinessValidationFailure([erro]);

            public static Result<T> InternalError(IList<string> errors) => new(errors, EStatusResult.ExceptionError);

            public static Result<T> InternalError(string erro) => InternalError([erro]);
        }
    }
}
