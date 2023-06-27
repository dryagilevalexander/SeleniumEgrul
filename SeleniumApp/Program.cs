using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V112.Audits;
using SeleniumApp;
using System.Data.Common;
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
        var partner = getPartnerData(inn);
        if(partner != null)
        {
            Console.WriteLine("Наименование контрагента: " + partner.Name);
            Console.WriteLine("Адрес: " + partner.Adress);
            Console.WriteLine("ОГРН: " + partner.Ogrn);
            Console.WriteLine("Дата присвоения ОГРН: " + partner.DateOgrn);
            Console.WriteLine("ИНН: " + partner.Inn);
            Console.WriteLine("КПП: " + partner.Kpp);
            Console.WriteLine("Тип руководителя: " + partner.DirType);
            Console.WriteLine("ФИО руководителя: " + partner.DirName);
        }
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

Partner? getPartnerData(string inn)
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

        Partner partner = new Partner
        {
            Adress = address,
            Name = partnerName,
            Ogrn = partnerParameters[0].Remove(0, partnerParameters[0].IndexOf(": ") + 2),
            DateOgrn = partnerParameters[1].Remove(0, partnerParameters[1].IndexOf(": ") + 2),
            Inn = partnerParameters[2].Remove(0, partnerParameters[2].IndexOf(": ") + 2),
            Kpp = partnerParameters[3].Remove(0, partnerParameters[3].IndexOf(": ") + 2),
            DirType = partnerParameters[4].Split(':')[0].Remove(0,1),
        DirName = partnerParameters[4].Remove(0, partnerParameters[4].IndexOf(": ") + 2)
        };
        
    return partner;
    }
    catch
    {
        Console.WriteLine("Контрагент не найден"); //Если блок try вызвал ошибку - контрагент не найден
        return null;
    }
}