﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF.Test
{
    [TestFixture]
    public class StringCutoffUnitTest
    {
        string text =
            @"1절 : 동해물과 백두산이 마르고 닳도록, 하느님이 보우하사 우리나라만세.
            (후렴) 무궁화 삼천리 화려강산 대한사람 대한으로 길이 보전하세.

            2절: 남산위에 저 소나무 철갑을 두른듯, 바람서리 불변함은 우리기상일세.
            (후렴) 무궁화 삼천리 화려강산 대한사람 대한으로 길이 보전하세.

            3절: 가을 하늘 공활한데, 높고 구름없이 밝은 달은 우리가슴 일편단심일세.
            (후렴) 무궁화 삼천리 화려강산 대한사람 대한으로 길이 보전하세.

            4절: 이 기상과 이 맘으로 충성을 다하여 괴로우나 즐거우나 나라사랑하세.
            (후렴) 무궁화 삼천리 화려강산 대한사람 대한으로 길이 보전하세.";

        [Test]
        public void FieldTest()
        {
            Maker maker = new Maker(text);
            int len = Encoding.UTF8.GetByteCount(maker.Name);
            Assert.IsTrue(len <= 256);
        }

        [Test]
        public void InventoryFormatTest()
        {
            InventoryFormat invf = new InventoryFormat();
            invf.Specification = text;
            invf.Memo = text;

            int len = Encoding.UTF8.GetByteCount(invf.Specification);
            Assert.IsTrue(len <= 256);

            len = Encoding.UTF8.GetByteCount(invf.Memo);
            Assert.IsTrue(len <= 256);
        }

        [Test]
        public void IOStockFormatTest()
        {
            IOStockFormat sfmt = new IOStockFormat();
            sfmt.Memo = text;

            int len = Encoding.UTF8.GetByteCount(sfmt.Memo);
            Assert.IsTrue(len <= 256);
        }
    }
}