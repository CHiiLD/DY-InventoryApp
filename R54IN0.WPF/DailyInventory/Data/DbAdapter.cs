#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0
{
    public class DbAdapter
    {
        private static DbAdapter _thiz;

        public DbAdapter()
        {
        }

        public static DbAdapter GetInstance()
        {
            if (_thiz == null)
                _thiz = new DbAdapter();
            return _thiz;
        }

        public async Task<bool> ConnectAsync()
        {
            return LexDb.GetDbInstance() != null;
        }

        public async Task<bool> DeleteAsync<TableT>(string id) where TableT : class
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException();
            var lexdb = LexDb.GetDbInstance();
            TableT item = lexdb.Table<TableT>().LoadByKey(id);
            if (item == null)
                return false;

            if (item is IOStockFormat)
                await CalculateDeledtedIOSFmtQuantity(item as IOStockFormat);
            return lexdb.Table<TableT>().DeleteByKey(id);
        }

        public async Task<bool> DeleteAsync<TableT>(TableT item) where TableT : class, IID
        {
            if (string.IsNullOrEmpty(item.ID))
                throw new ArgumentNullException();
            if (item is IOStockFormat)
                await CalculateDeledtedIOSFmtQuantity(item as IOStockFormat);
            var lexdb = LexDb.GetDbInstance();
            return lexdb.Table<TableT>().Delete(item);
        }

        public async Task InsertAsync<TableT>(TableT item) where TableT : class, IID
        {
            if (item == null)
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(item.ID))
                item.ID = Guid.NewGuid().ToString();
            if (item is IOStockFormat)
                await CalculateAddedIOSFmtQuantity(item as IOStockFormat);
            var lexdb = LexDb.GetDbInstance();
            lexdb.Table<TableT>().Save(item);
        }

        public async Task UpdateAsync<TableT>(TableT item, params string[] properties) where TableT : class, IID
        {
            if (item == null)
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(item.ID))
                item.ID = Guid.NewGuid().ToString();
            if (item is IOStockFormat)
                await CalculateModifiedIOSFmtQuantity(item as IOStockFormat);
            var lexdb = LexDb.GetDbInstance();
            lexdb.Table<TableT>().Save(item);
        }

        public async Task<IEnumerable<TableT>> SelectAllAsync<TableT>() where TableT : class
        {
            var lexdb = LexDb.GetDbInstance();
            var items = lexdb.Table<TableT>().LoadAll();
            return items as IEnumerable<TableT>;
        }

        /// <summary>
        /// 데이터베이스의 쿼리 기능을 지원합니다. 복수의 명령어를 조합하여 사용할 수 있습니다.
        /// </summary>
        /// <typeparam name="TableT"></typeparam>
        /// <typeparam name="KeyT"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TableT>> QueryAsync<TableT>(params object[] commandOptions) where TableT : class
        {
            if (commandOptions == null || commandOptions.Count() == 0 || !(commandOptions.ElementAt(0) is DbCommand))
                throw new ArgumentException();

            List<List<object>> list = new List<List<object>>();
            int idx = -1;
            foreach (var arg in commandOptions)
            {
                if (arg is DbCommand)
                {
                    idx++;
                    list.Add(new List<object>());
                }
                list[idx].Add(arg);
            }

            IEnumerable<TableT> source = LexDb.GetDbInstance().LoadAll<TableT>();

            foreach (var item in list)
            {
                DbCommand cmd = (DbCommand)item[0];
                object[] args = item.GetRange(1, item.Count() - 1).ToArray();

                DbCommand cmdCpy = cmd;
                if (cmd.HasFlag(DbCommand.OR_EQUAL))
                    cmdCpy ^= DbCommand.OR_EQUAL;

                switch (cmdCpy)
                {
                    case DbCommand.WHERE:
                        source = ExecuteWhereCommand(cmd, source, args);
                        break;

                    case DbCommand.ASCENDING:
                    case DbCommand.DESCENDING:
                        source = ExecuteOrderByCommand(cmd, source, args);
                        break;

                    case DbCommand.BETWEEN:
                        source = ExecuteBetweenOperCommand(cmd, source, args);
                        break;

                    case DbCommand.IS_GRETER_THEN:
                    case DbCommand.IS_LESS_THEN:
                        source = ExecuteCompareOperCommand(cmd, source, args);
                        break;

                    case DbCommand.LIMIT:
                        source = ExecuteGetRangeCommand(cmd, source, args);
                        break;

                    default:
                        throw new Exception("인식할 수 없는 DbCommand 명령어");
                }
            }
            return source;
        }

        private async Task CalculateAddedIOSFmtQuantity(IOStockFormat iosfmt)
        {
            int qty = iosfmt.Quantity;
            IOStockFormat near = null;
            IEnumerable<IOStockFormat> queryResult = await QueryAsync<IOStockFormat>(
                        DbCommand.WHERE, "InventoryID", iosfmt.InventoryID,
                        DbCommand.IS_LESS_THEN, "Date", iosfmt.Date,
                        DbCommand.DESCENDING, "Date",
                        DbCommand.LIMIT, 1);
            if (queryResult != null && queryResult.Count() == 1)
                near = queryResult.Single();
            var lexdb = LexDb.GetDbInstance();
            InventoryFormat infmt = lexdb.Table<InventoryFormat>().LoadByKey(iosfmt.InventoryID);
            ///잔여수량과 재고수량 계산
            switch (iosfmt.StockType)
            {
                case IOStockType.INCOMING:
                    iosfmt.RemainingQuantity = (near == null) ? qty : near.RemainingQuantity + qty;
                    infmt.Quantity = (infmt == null) ? qty : infmt.Quantity + qty;
                    break;

                case IOStockType.OUTGOING:
                    iosfmt.RemainingQuantity = (near == null) ? -qty : near.RemainingQuantity - qty;
                    infmt.Quantity = (infmt == null) ? -qty : infmt.Quantity - qty;
                    break;
            }
            lexdb.Save(infmt);
            ///잔여 수량 동기화 및 저장
            IEnumerable<IOStockFormat> formats = await QueryAsync<IOStockFormat>(
                DbCommand.WHERE, "InventoryID", iosfmt.InventoryID,
                DbCommand.IS_GRETER_THEN, "Date", iosfmt.Date,
                DbCommand.ASCENDING, "Date");
            if (qty == 0 || formats.Count() == 0)
            {
                qty = iosfmt.StockType == IOStockType.OUTGOING ? -qty : qty;
                foreach (IOStockFormat fmt in formats)
                {
                    fmt.RemainingQuantity += qty;
                    lexdb.Save(fmt);
                }
            }
        }

        private async Task CalculateModifiedIOSFmtQuantity(IOStockFormat iosfmt)
        {
            int qty = iosfmt.Quantity;
            var lexdb = LexDb.GetDbInstance();
            InventoryFormat infmt = lexdb.Table<InventoryFormat>().LoadByKey(iosfmt.InventoryID);
            IOStockFormat origin = lexdb.Table<IOStockFormat>().LoadByKey(iosfmt.ID);
            switch (iosfmt.StockType)
            {
                case IOStockType.INCOMING:
                    iosfmt.RemainingQuantity = origin.RemainingQuantity + qty - origin.Quantity;
                    infmt.Quantity = infmt.Quantity + qty - origin.Quantity;
                    break;
                case IOStockType.OUTGOING:
                    iosfmt.RemainingQuantity = origin.RemainingQuantity + origin.Quantity - qty;
                    infmt.Quantity = infmt.Quantity + origin.Quantity - qty;
                    break;
            }
            lexdb.Save(infmt);
            IEnumerable<IOStockFormat> formats = await QueryAsync<IOStockFormat>(
                DbCommand.WHERE, "InventoryID", iosfmt.InventoryID,
                DbCommand.IS_GRETER_THEN, "Date", iosfmt.Date,
                DbCommand.ASCENDING, "Date");
            if (qty == 0 || formats.Count() == 0)
            {
                qty = iosfmt.StockType == IOStockType.OUTGOING ? origin.Quantity - qty : qty - origin.Quantity;
                foreach (IOStockFormat fmt in formats)
                {
                    fmt.RemainingQuantity += qty;
                    lexdb.Save(fmt);
                }
            }
        }

        private Task CalculateDeledtedIOSFmtQuantity(IOStockFormat iosfmt)
        {
            throw new NotSupportedException();
        }

        private IEnumerable<TableT> ExecuteWhereCommand<TableT>(DbCommand cmd, IEnumerable<TableT> source, params object[] args)
        {
            var col = args.ElementAtOrDefault(0);
            var key = args.ElementAtOrDefault(1);
            if (col == null || !(col is string) || key == null || !(key is IComparable))
                throw new Exception();

            string column = col as string;
            IComparable target = key as IComparable;

            source = source.Where(x => target.CompareTo(x.GetType().GetProperty(column).GetValue(x)) == 0);
            return source;
        }

        private IEnumerable<TableT> ExecuteGetRangeCommand<TableT>(DbCommand cmd, IEnumerable<TableT> source, params object[] args)
        {
            var cnt = args.ElementAtOrDefault(0);
            if (cnt == null || !(cnt is int))
                throw new Exception();
            int count = (int)cnt;
            if (source.Count() > count)
                source = source.ToList().GetRange(0, count);
            return source;
        }

        private IEnumerable<TableT> ExecuteOrderByCommand<TableT>(DbCommand cmd, IEnumerable<TableT> source, params object[] args)
        {
            var col = args.ElementAtOrDefault(0);
            if (col == null || !(col is string))
                throw new Exception();
            string column = col as string;
            if (cmd.HasFlag(DbCommand.ASCENDING))
                source = source.OrderBy(x => x.GetType().GetProperty(column).GetValue(x));
            else if (cmd.HasFlag(DbCommand.DESCENDING))
                source = source.OrderByDescending(x => x.GetType().GetProperty(column).GetValue(x));
            return source;
        }

        private IEnumerable<TableT> ExecuteCompareOperCommand<TableT>(DbCommand cmd, IEnumerable<TableT> source, params object[] args)
        {
            object col = args.ElementAtOrDefault(0);
            object tar = args.ElementAtOrDefault(1);
            if (tar == null || !(tar is IComparable) || col == null || !(col is string))
                throw new Exception();

            IComparable target = tar as IComparable;
            string column = col as string;

            if (cmd.HasFlag(DbCommand.IS_LESS_THEN))
            {
                if (cmd.HasFlag(DbCommand.OR_EQUAL))
                    source = source.Where(x => 0 <= target.CompareTo(x.GetType().GetProperty(column).GetValue(x)));
                else
                    source = source.Where(x => 0 < target.CompareTo(x.GetType().GetProperty(column).GetValue(x)));
            }
            else if (cmd.HasFlag(DbCommand.IS_GRETER_THEN))
            {
                if (cmd.HasFlag(DbCommand.OR_EQUAL))
                    source = source.Where(x => target.CompareTo(x.GetType().GetProperty(column).GetValue(x)) <= 0);
                else
                    source = source.Where(x => target.CompareTo(x.GetType().GetProperty(column).GetValue(x)) < 0);
            }
            return source;
        }

        private List<TableT> ExecuteBetweenOperCommand<TableT>(DbCommand cmd, IEnumerable<TableT> source, params object[] args)
        {
            List<TableT> result = new List<TableT>();
            object col = args.ElementAtOrDefault(0);
            object min = args.ElementAtOrDefault(1);
            object max = args.ElementAtOrDefault(2);

            if (col == null || min == null || max == null || !(col is string))
                throw new ArgumentException();

            string column = col as string;

            foreach (var src in source)
            {
                IComparable target = src.GetType().GetProperty(column).GetValue(src) as IComparable;
                if (target == null)
                    throw new Exception();
                if (cmd.HasFlag(DbCommand.OR_EQUAL))
                {
                    if (0 <= target.CompareTo(min) && target.CompareTo(max) <= 0)
                        result.Add(src);
                }
                else
                {
                    if (0 < target.CompareTo(min) && target.CompareTo(max) < 0)
                        result.Add(src);
                }
            }
            return result;
        }
    }
}