using Management_Hotel_2025.Serives.AuthenSerive;

namespace TEST
{
    public class TestValidationAuthen
    {
        [Theory]
        [InlineData("123456789PhamTrungDuc", true)]
        [InlineData("123456789phamtrungduc", false)]
        [InlineData("12345", false)]
        [InlineData("", false)]
        [InlineData("654321Vcl", true)]
        public void Test1(string input, bool expect)
        {
            // Rarrang
            ValidationAuthen validationAuthen = new ValidationAuthen();

            // Act  
            var reuslt = validationAuthen.ValidatePassword(input);

            // Assert
            Assert.Equal(expect, reuslt);
        }
    }
}