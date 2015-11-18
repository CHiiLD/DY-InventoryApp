using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lex.Db;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public interface IClass
    {
        string UUID { get; set; }
        int Value { get; set; }
    }

    public class ClassA : IClass
    {
        public ClassA() { }
        public ClassA(int value)
        {
            Value = value;
            UUID = Guid.NewGuid().ToString();
        }
        public string UUID { get; set; }
        public int Value { get; set; }
    }

    public class ClassB
    {
        public ClassB() { }
        public ClassB(string value)
        {
            Value = value;
            UUID = Guid.NewGuid().ToString();
        }
        public string UUID { get; set; }
        public string Value { get; set; }
    }

    public class ClassC
    {
        public ClassC() { }
        public ClassC(DateTime date)
        {
            Date = date;
            UUID = Guid.NewGuid().ToString();
        }
        public DateTime Date { get; set; }
        public string UUID { get; set; }
    }

    public class ClassD
    {
        public ClassD() { }
        public ClassD(int? value)
        {
            Value = value;
            UUID = Guid.NewGuid().ToString();
        }
        public int? Value { get; set; }
        public string UUID { get; set; }
    }

    public class ClassE
    {
        public ClassE() { }
        public ClassE(List<string> value)
        {
            Value = value;
            UUID = Guid.NewGuid().ToString();
        }
        public List<string> Value { get; set; }
        public string UUID { get; set; }
    }

    [TestClass]
    public class LexDBTest
    {
        [TestMethod]
        public void MultiplexClassUseInTable()
        {
            //Table 클래스 중복 테스트하기
            using (DbInstance db = new DbInstance("LexDBTest1.db"))
            {
                db.Map<ClassA>().Automap(i => i.UUID).WithIndex("Value", i => i.Value);
                db.Map<ClassB>().Automap(i => i.UUID).WithIndex("Value", i => i.Value);
                db.Map<ClassC>().Automap(i => i.UUID).WithIndex("Date", i => i.Date);
                db.Map<ClassD>().Automap(i => i.UUID).WithIndex("Value", i => i.Value);
                db.Map<ClassE>().Automap(i => i.UUID).WithIndex("Value", i => i.Value);
                db.Initialize();
                db.Purge();

                //테이블 추가 및 레코드 저장
                db.Save(new ClassA(1), new ClassA(2), new ClassA(3));
                ClassA[] classA = db.LoadAll<ClassA>();
                foreach (var a in classA)
                    Console.WriteLine(a.Value);

                db.Save(new ClassB("A"), new ClassB("B"), new ClassB("C"));
                ClassB[] classB = db.LoadAll<ClassB>();
                foreach (var b in classB)
                    Console.WriteLine(b.Value);

                db.Save(new ClassA(1), new ClassA(2), new ClassA(3));
                classA = db.LoadAll<ClassA>();
                foreach (var a in classA)
                    Console.WriteLine(a.Value);

                Assert.AreEqual(db.Count<ClassA>(), 6);
                Assert.AreEqual(db.Count<ClassB>(), 3);

                //테이블 삭제
                db.Purge<ClassB>();
                classB = db.LoadAll<ClassB>();

                Assert.AreEqual(db.Count<ClassB>(), 0);
                Assert.AreEqual(classB.Length, 0);
                Assert.AreEqual(db.Count<ClassA>(), 6);

                //삭제
                var classX = new ClassA(100);
                db.Save(classX);
                Assert.AreEqual(db.Count<ClassA>(), 7);
                db.Delete(classX);
                Assert.AreEqual(db.Count<ClassA>(), 6);

                //키에 의한 삭제
                db.Save(classX);
                Assert.AreEqual(db.Count<ClassA>(), 7);
                db.DeleteByKey<ClassA, string>(classX.UUID);
                Assert.AreEqual(db.Count<ClassA>(), 6);

                //키에 의한 쿼리
                db.Save(classX);
                ClassA findA = db.LoadByKey<ClassA, string>(classX.UUID);
                Assert.AreEqual(findA.UUID, classX.UUID);
                Assert.AreEqual(findA.Value, classX.Value);

                //편집
                db.Purge<ClassA>();
                for (int i = 0; i < 10; i++)
                {
                    ClassA newA = new ClassA();
                    newA.Value = i;
                    newA.UUID = classX.UUID;
                    db.Save(newA);
                }
                Assert.AreEqual(db.Count<ClassA>(), 1);
                Assert.AreEqual(db.LoadByKey<ClassA, string>(classX.UUID).Value, 9);

                //DateTime 저장 테스트
                ClassC newC = new ClassC(DateTime.Now);
                db.Save(newC);
                ClassC findC = db.LoadByKey<ClassC, string>(newC.UUID);
                Assert.AreEqual(newC.Date, findC.Date);

                //Nullable 객체 저장 테스트
                ClassD newD = new ClassD(1);
                db.Save(newD);
                ClassD findD = db.LoadByKey<ClassD, string>(newD.UUID);
                Assert.AreEqual(newD.Value, findD.Value);

                newD = new ClassD(null);
                db.Save(newD);
                findD = db.LoadByKey<ClassD, string>(newD.UUID);
                Assert.AreEqual(newD.Value, findD.Value);

                //제네릭 객체 저장 테스트
                var list = new List<string>() { "a", "b", "c" };
                ClassE newE = new ClassE(list);
                db.Save(newE);
                ClassE findE = db.LoadByKey<ClassE, string>(newE.UUID);
                Assert.AreEqual(list.SequenceEqual(findE.Value), true);

                //Null 반응 테스트
                findE = db.LoadByKey<ClassE, string>(null);
                Assert.AreEqual(findE, null);
            }
        }
    }
}