﻿using System.Collections;

namespace R54IN0
{
    /// <summary>
    /// 화폐
    /// </summary>
    public class Currency : IField
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}