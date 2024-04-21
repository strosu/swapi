namespace Swapi.Tests
{
    public class UrlUtilsTests
    {
        [Fact]
        public void ParsesId_Extracts_Correctly()
        {
            var id = UrlUtils.ConvertToId("https://swapi.dev/api/species/1/");
            Assert.Equal(1, id);
        }
    }
}
