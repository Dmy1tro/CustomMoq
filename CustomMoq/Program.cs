using CustomMoq.ILGenerationMoq;

RuntimeMockTest();
ILGenerationMoqTest();

void RuntimeMockTest()
{
    var converterMock = CustomMoq.RuntimeCompilationMoq.Moq.For<IConverter>();
    converterMock.Setup(c => c.ConvertToInt(null)).Returns(1234);
    converterMock.Setup(c => c.ConvertToString(null)).Returns("qwerty");

    // Get mocked object
    var converter = converterMock.Object;

    // Use mocked methods.
    var res1 = converter.ConvertToInt("")!;
    var res2 = converter.ConvertToString("")!;

    // Check that return values are correct.
    var isTrue = res1 == 1234 && res2 == "qwerty";

    // Just should work without any errors.
    converter.SaveData("");
}

void ILGenerationMoqTest()
{
    var converter = CustomMoq.ILGenerationMoq.Moq.For<IConverter>();

    converter.ConvertToInt("").Returns(1234);
    converter.ConvertToString("").Returns("qwerty");

    var res1 = converter.ConvertToInt("");
    var res2 = converter.ConvertToString("");

    // Check that return values are correct.
    var isTrue = res1 == 1234 && res2 == "qwerty";

    // Just should work without any errors.
    converter.SaveData("");
}


