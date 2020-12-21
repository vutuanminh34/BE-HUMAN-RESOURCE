using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.ViewModels
{
    public class PersonViewModel:BaseViewModel
    {
        private Person _personInfo;
        public PersonViewModel()
        {
            if (_personInfo == null)
                _personInfo = new Person();
            if (BaseModel == null)
                BaseModel = new BaseModel();
        }
        public BaseModel BaseModel { get; set; }

        public Person PersonInfo 
        {
            get { return _personInfo; }
            set { _personInfo = value; }
        } 
    }
}
