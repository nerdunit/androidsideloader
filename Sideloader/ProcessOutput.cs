namespace AndroidSideloader
{
    public class ProcessOutput
    {
        public string Output;
        public string Error;

        public ProcessOutput(string output = "", string error = "")
        {
            Output = output;
            Error = error;
        }

        public static ProcessOutput operator +(ProcessOutput a, ProcessOutput b)
        {
            return new ProcessOutput(a.Output + b.Output, a.Error + b.Error);
        }
    }
}
