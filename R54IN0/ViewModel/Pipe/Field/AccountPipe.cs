using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class AccountPipe : FieldPipe<Account>
    {
        public AccountPipe(Account seller)
            : base(seller)
        {

        }

        public string Delegator
        {
            get
            {
                return Field.Delegator;
            }
            set
            {
                Field.Delegator = value;
                Field.Save<Account>();
                OnPropertyChanged("Delegator");
            }
        }

        public string PhoneNumber
        {
            get
            {
                return Field.PhoneNumber;
            }
            set
            {
                Field.PhoneNumber = value;
                Field.Save<Account>();
                OnPropertyChanged("PhoneNumber");
            }
        }

        public string MobileNumber
        {
            get
            {
                return Field.MobileNumber;
            }
            set
            {
                Field.MobileNumber = value;
                Field.Save<Account>();
                OnPropertyChanged("MobileNumber");
            }
        }
    }
}