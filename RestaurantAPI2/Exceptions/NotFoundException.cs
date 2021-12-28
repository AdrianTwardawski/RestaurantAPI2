using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2.Exceptions
{
    /*W ten sposób jeśli klient będzie chciał się odnieść do zasobu na serverze, który nie istnieje,
     *to zamiast zwracać do kontrolera informacje że dany zasób nie istnieje przez flage boolean,
     *będzie bezpośrednio wyrzucać wyjątek w serwisie, który będzie odwpowiedzialny za znalezienie takiego zasobu
     *i następnie ten wyjatek będzie odpowiednio obsłużony w ErrorHandlingMiddleware dzięki temu implementacje
     *serwisu jak i implementacje akcji w kontrolerze zostają uproszczone przez co kod jest bardziej czytelny */
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) 
        {

        }
    }
}
