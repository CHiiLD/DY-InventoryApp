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
                OnPropertyChanged("MobileNumber");
            }
        }
    }
}