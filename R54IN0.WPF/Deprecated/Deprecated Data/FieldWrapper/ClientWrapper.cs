﻿namespace R54IN0
{
    public class ClientWrapper : Observable<Client>
    {
        public ClientWrapper()
            : base()
        {
        }

        public ClientWrapper(Client client)
            : base(client)
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
                Field.Save<Client>();
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
                Field.Save<Client>();
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
                Field.Save<Client>();
                OnPropertyChanged("MobileNumber");
            }
        }
    }
}