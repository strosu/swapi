namespace Swapi
{
    public class Utils
    {
        public static int RoundUpDivision(int numerator, int denumerator) 
            => (numerator + denumerator - 1) / denumerator;
    }
}
