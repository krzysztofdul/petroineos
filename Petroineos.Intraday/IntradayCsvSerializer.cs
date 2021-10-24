namespace Petroineos.Intraday
{
    public class IntradayCsvSerializer : ICsvSerializer
    {
        public string GetHeader()
        {
            return "Local Time, Volume";
        }

        public string GetRow(string time, decimal volume)
        {
            return $"{time},{volume}";
        }
    }

    public interface ICsvSerializer
    {
        string GetHeader();
        string GetRow(string time, decimal volume);
    }
}