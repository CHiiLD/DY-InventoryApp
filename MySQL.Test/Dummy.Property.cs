using System.Collections.Generic;

namespace MySQL.Test
{
    public partial class Dummy
    {
        private string[] _clientNames = new string[]
        {
            "예시스템", "엠비콘넥터", "이오텍", "네오테크", "여명", "춘일", "덕성전기", "풍림", "춘일", "영도케이블",
            "동인", "미래조명", "제이엠시스"
        };

        private string[] _humanNames = new string[]
        {
            "Emma",
            "Peg",
            "Wai",
            "Zack",
            "Stanford",
            "Jayme",
            "Carter",
            "Laila",
            "Jeanna" };

        private string[] _makerNames = new string[]
        {
            "LG",
            "선트로닉스",
            "LSIS",
            "미쯔비시",
            "한영",
            "위너스",
            "캐논",
            "슈나이더",
            "건홍",
            "세기비즈",
            "동아",
            "금호전기",
            "장안",
            "KD POWER",
            "번개표",
            "Q-LIGHT",
        };

        private string[] _measureNames = new string[]
        {
            "EA",
            "BOX",
            "PIC",
            "SET",
        };

        private string[] _warehouseNames = new string[]
        {
            "1층 적재 A구역",
            "1층 적재 B구역",
            "1층 적재 C구역",
            "1층 적재 D구역",
            "연구실",
            "사무실",
            "회사 창고",
        };

        private Dictionary<string, string[]> _productNames = new Dictionary<string, string[]>()
        {
            { "플러그", new string[] { "EPP2-21010" } },
            { "커버", new string[] { "HP119E", "HP111", "HP117" } },
            { "버섯형 누름 버튼", new string[] { "ZB5 AC3" } },
            { "버튼 단자부", new string[] { "ZB5-AZ101", "ZB5-AZ104" } },
            { "설렉터 스위치", new string[] { "ZB5AS844" } },
            { "Key 스위치", new string[] { "ZB5 AG4" } },
            { "PBL", new string[] { "ZB5 AW233" } },
            { "단자부", new string[] { "ZB5 AW343", "ZB5 AWBB451" } },
            { "SWTICH BOX", new string[] {
                "KCB-304D", "KCB-303D", "KCB-302D", "KCB-301D",
                "KCB-300D", "KCB-393D", "KCB-200D", "KCB-232D",
                "KCB-104D", "KCB-103D", "KCB-102D", "KCB-101D",} },
            { "등기구", new string[] { "LFX12", "LFX06" } },
            { "판넬 직부등", new string[] { "KCL-100" } },
        };

        private string[] _projectNames = new string[]
        {
            "DY1234",
            "DY1235",
            "DY1236",
            "DY1237",
            "DY1238",
            "DY1239",
        };
    }
}