namespace Cheetah.CLI
{
    public class Arguments
    {
        public string RawText { get; }

        public string[] ArgumentList { get; }

        public Arguments(string text)
        {
            RawText = text;
            ArgumentList = text.Split(' ');
        }

        public int? GetInt(int pos)
        {
            if(int.TryParse(ArgumentList[pos], out int result))
            {
                return result;
            }
            return null;
        }
    }
}