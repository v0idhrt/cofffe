//using AIcontrolComputer.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIcontrolComputer.Models
{

    public interface ILanguageModelsModule
    {
        string GeterateResponses(string input);
    }
    public class LanguageModelsModule : ILanguageModelsModule
    {
        public string GeterateResponses(string input)
        {
            
            
            return "Не готово";
        }
    }
}
