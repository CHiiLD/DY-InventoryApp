﻿using R54IN0.WPF;
using System.ComponentModel;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace R54IN0.WPF
{
    public class ObservableInventory : IInventoryFormat, IObservableInventoryProperties, ICanUpdate
    {
        private InventoryFormat _fmt;
        private bool _canUpdate = true;
        protected Observable<Product> product;
        protected Observable<Maker> maker;
        protected Observable<Measure> measure;
        protected PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged -= value;
                propertyChanged += value;
            }
            remove
            {
                propertyChanged -= value;
            }
        }

        public ObservableInventory()
        {
            _fmt = new InventoryFormat();
        }

        public ObservableInventory(Observable<Product> product, string specification, int quantity, string memo,
            Observable<Maker> maker = null, Observable<Measure> measure = null) : this()
        {
            this.product = product;
            _fmt.ProductID = product.ID;
            _fmt.Specification = specification;
            _fmt.Quantity = quantity;
            _fmt.Memo = memo;
            if (maker != null)
            {
                _fmt.MakerID = maker.ID;
                this.maker = maker;
            }
            if (measure != null)
            {
                _fmt.MeasureID = measure.ID;
                this.measure = measure;
            }
        }

        public ObservableInventory(InventoryFormat fmt)
        {
            _fmt = fmt;
            InitializeProperties(fmt);
        }

        public virtual InventoryFormat Format
        {
            get
            {
                return _fmt;
            }
            set
            {
                _fmt = value;
                InitializeProperties(_fmt);
                NotifyPropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// 제품의 규격 이름
        /// </summary>
        public virtual string Specification
        {
            get
            {
                return _fmt.Specification;
            }
            set
            {
                if (_fmt.Specification != value)
                {
                    _fmt.Specification = value;
                    NotifyPropertyChanged("Specification");
                }
            }
        }

        /// <summary>
        /// 적재 수량
        /// </summary>
        public virtual int Quantity
        {
            get
            {
                return _fmt.Quantity;
            }
            set
            {
                if (_fmt.Quantity != value)
                {
                    _fmt.Quantity = value;
                    NotifyPropertyChanged("Quantity");
                }
            }
        }

        /// <summary>
        /// 비고
        /// </summary>
        public virtual string Memo
        {
            get
            {
                return _fmt.Memo;
            }
            set
            {
                if (_fmt.Memo != value)
                {
                    _fmt.Memo = value;
                    NotifyPropertyChanged("Memo");
                }
            }
        }

        /// <summary>
        /// 제품
        /// </summary>
        public virtual Observable<Product> Product
        {
            get
            {
                return product;
            }
            set
            {
                if (product != value)
                {
                    _fmt.ProductID = value != null ? value.ID : null;
                    product = value;
                    NotifyPropertyChanged("Product");
                }
            }
        }

        /// <summary>
        /// 단위
        /// </summary>
        public virtual Observable<Measure> Measure
        {
            get
            {
                return measure;
            }
            set
            {
                if (measure != value)
                {
                    _fmt.MeasureID = value != null ? value.ID : null;
                    measure = value;
                    NotifyPropertyChanged("Measure");
                }
            }
        }

        /// <summary>
        /// 제조사
        /// </summary>
        public virtual Observable<Maker> Maker
        {
            get
            {
                return maker;
            }
            set
            {
                if (maker != value)
                {
                    _fmt.MakerID = value != null ? value.ID : null;
                    maker = value;
                    NotifyPropertyChanged("Maker");
                }
            }
        }

        public virtual string ID
        {
            get
            {
                return _fmt.ID;
            }
            set
            {
                _fmt.ID = value;
            }
        }

        public string MakerID
        {
            get
            {
                return _fmt.MakerID;
            }
            set
            {
                if (_fmt.MakerID != value)
                    Maker = DataDirector.GetInstance().SearchField<Maker>(value);
            }
        }

        public string MeasureID
        {
            get
            {
                return _fmt.MeasureID;
            }
            set
            {
                if (_fmt.MeasureID != value)
                    Measure = DataDirector.GetInstance().SearchField<Measure>(value);
            }
        }

        public string ProductID
        {
            get
            {
                return _fmt.ProductID;
            }
            set
            {
                if (_fmt.MeasureID != value)
                    Product = DataDirector.GetInstance().SearchField<Product>(value);
            }
        }

        public bool CanUpdate
        {
            get
            {
                return _canUpdate;
            }
            set
            {
                _canUpdate = value;
            }
        }

        protected void InitializeProperties(InventoryFormat fmt)
        {
            var ofd = DataDirector.GetInstance();
            product = ofd.SearchField<Product>(fmt.ProductID);
            measure = ofd.SearchField<Measure>(fmt.MeasureID);
            maker = ofd.SearchField<Maker>(fmt.MakerID);
        }

        public virtual void NotifyPropertyChanged(string name)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));

            if (string.IsNullOrEmpty(name))
                return;

            string[] s = new string[] { nameof(this.Product), nameof(this.Maker), nameof(this.Measure) };
            if (s.Any(x => x == name))
                name = name.Insert(name.Length, "ID");

            if (ID == null)
                DataDirector.GetInstance().AddInventory(this);
            else if (CanUpdate)
                DataDirector.GetInstance().DB.Update(Format, name);
        }

        public void Sync()
        {
            InventoryFormat fmt = DataDirector.GetInstance().DB.Select<InventoryFormat>(ID);
            if (fmt != null)
                Format = fmt;
        }
    }
}