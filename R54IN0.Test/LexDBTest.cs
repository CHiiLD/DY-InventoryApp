﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Lex.Db;

namespace R54IN0.Test
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

    public class ClassF
    {
        public ClassF() { }
        public ClassF(Dictionary<string, int> value)
        {
            Value = value;
            UUID = Guid.NewGuid().ToString();
        }
        public Dictionary<string, int> Value { get; set; }
        public string UUID { get; set; }
    }

    public class Node
    {
        public string UUID{ get; set;} 
        public Node Link { get; set; }
        public string Str { get; set; }

        public Node()
        {
            UUID = Guid.NewGuid().ToString();
        }
    }

    public class Class
    {
        public string UUID { get; set; }
        public ClassF classF { get; set; }
    }

    [TestClass]
    [Ignore]
    public class LexDBTest
    {
        [TestMethod]
        public void MultiplexClassUseInDb()
        {
            //Table 클래스 중복 테스트하기
            using (DbInstance db = new DbInstance("LexDBTest1.db"))
            {
                db.Map<ClassA>().Automap(i => i.UUID).WithIndex("Value", i => i.Value);
                db.Map<ClassB>().Automap(i => i.UUID).WithIndex("Value", i => i.Value);
                db.Map<ClassC>().Automap(i => i.UUID).WithIndex("Date", i => i.Date);
                db.Map<ClassD>().Automap(i => i.UUID).WithIndex("Value", i => i.Value);
                db.Map<ClassE>().Automap(i => i.UUID).WithIndex("Value", i => i.Value);
                db.Map<ClassF>().Automap(i => i.UUID).WithIndex("Value", i => i.Value);
                db.Map<Class>().Automap(i => i.UUID).WithIndex("classF", i => i.classF);
                //db.Map<Node>().Automap(i => i.UUID).WithIndex("Str", i => i.Str).WithIndex("Link", i => i.Link);
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

                newE.Value.Add("d");
                db.Save(newE);
                findE = db.LoadByKey<ClassE, string>(newE.UUID);
                Assert.AreEqual(newE.Value.SequenceEqual(findE.Value), true);

                //Null 반응 테스트
                findE = db.LoadByKey<ClassE, string>(null);
                Assert.AreEqual(findE, null);

                //dictionary..
                var dic = new Dictionary<string, int>() { { "a", 1 }, { "b", 2 } };
                ClassF newF = new ClassF(dic);
                db.Save(newF);
                ClassF findF = db.LoadByKey<ClassF, string>(newF.UUID);
                Assert.AreEqual(dic.SequenceEqual(findF.Value), true);

                //newF.Value = new Dictionary<string, int>(dic);
                newF.Value["a"] = 10;
                db.Save(newF);
                findF = db.LoadByKey<ClassF, string>(newF.UUID);
                Assert.AreEqual(newF.Value.SequenceEqual(findF.Value), true);

                //트리형 Node
                Node root = new Node();
                Node i1 = new Node();
                Node i2 = new Node();

                Node i11 = new Node();
                Node i12 = new Node();
                Node i111 = new Node();

                i11.Link = i111;
                i1.Link = i11;
                i1.Link = i12;
                root.Link = i1;
                root.Link = i2;

                db.Save(root);
                Node node = db.LoadByKey<Node>(root.UUID);
            }
        }
    }
}