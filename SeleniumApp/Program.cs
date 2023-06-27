using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V112.Audits;
using System.Reflection.Metadata;
using System.Security.Authentication;
using static System.Net.Mime.MediaTypeNames;

Console.WriteLine("Program start");
Console.Write("Введите ИНН: ");
string? inn = Console.ReadLine();
if(inn!=null)
{
    if(inn.Length==10)
    {
        getPartnerData(inn);
    }
    else
    {
        Console.WriteLine("Введен некорректный ИНН. Перезапустите программу");
    }
}
else
{
    Console.WriteLine("Вы не ввели значение. Перезапустите программу");
}

Console.WriteLine("Program end");

void getPartnerData(string inn)
{
    ChromeOptions options = new ChromeOptions(); //создаем экземпляр хром
    options.PageLoadStrategy = PageLoadStrategy.Normal; //ожидаем нормальной загрузки страниц
    options.AddArgument("--headless"); //скрываем браузер
    IWebDriver chrome = new OpenQA.Selenium.Chrome.ChromeDriver(options); //создаём и инициализируем объект драйвера
    chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); //устанавливаем неявное ожидание в 5 секунд
    chrome.Navigate().GoToUrl("https://egrul.nalog.ru"); //открываем ссылку
    chrome.FindElement(By.Id("query")).SendKeys(inn); //ищем поле по id и вводим полученный ИНН
    chrome.FindElement(By.Id("btnSearch")).Click(); //Нажимаем на кнопку "Поиск"
    try
    {
        string partnerName = chrome.FindElement(By.ClassName("op-excerpt")).Text; //Ищем элемент с соответствующим классом и получаем текст
        string partnerData = chrome.FindElement(By.ClassName("res-text")).Text;   //Получаем данные из элемента с соответствующим классом
        string address = partnerData.Substring(0, partnerData.IndexOf(", ОГРН")); //Получаем адрес
        partnerData = partnerData.Remove(0, address.Length + 1);                  //Удаляем адрес для упрощения разбора строки
        string[] partnerParameters = partnerData.Split(',');                      //Разбираем строку, разделить запятая
        Console.WriteLine(partnerName);
        Console.WriteLine(address);
        foreach (var parameter in partnerParameters)
        {
            Console.WriteLine(parameter.Remove(0, parameter.IndexOf(": ") + 2));  //Выводим данные контрагента, удаляя ненужные символы
        }
    }
    catch
    {
        Console.WriteLine("Контрагент не найден"); //Если блок try вызвал ошибку - контрагент не найден
    }
}