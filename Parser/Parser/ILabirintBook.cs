using DataLayer.Entities;

namespace Parser.Parser
{
    public interface ILabirintBook
    {
        // интерфейс для сервиса парсера
        int GetBookUrl(); //получить последний использовавшийся url книги

        void Update(string lastUrl); //обновить последний url книги в базе данных

        ParserLastUrl GetParseSettings(); // возвращает данные о парсере

        void UpdateSettings(ParserLastUrl parserLastUrl); // обновляет настройки парсера
    }
}
