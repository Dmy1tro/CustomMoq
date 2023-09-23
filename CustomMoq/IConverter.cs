
public interface IConverter
{
    int ConvertToInt(string str);
    string ConvertToString(object obj);
    void SaveData(object obj);
}
