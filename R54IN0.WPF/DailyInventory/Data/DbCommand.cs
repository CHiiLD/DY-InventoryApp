using System;

namespace R54IN0
{
    [Flags]
    public enum DbCommand
    {
        #region option
        /// <summary>
        /// 쿼리시, OR연산(<=, >=)을 추가합니다.
        /// </summary>
        OR_EQUAL = 1 << 0,
        #endregion

        #region compare
        /// <summary>
        /// 최소값과 최대값 사이의 의미를 가짐 OR_EQUAL와 함께 쓰임
        /// </summary>
        BETWEEN = 1 << 10,
        /// <summary>
        /// 미만 
        /// OR_EQUAL와 함께 쓰일 경우 작거나 같다 의 의미를 가짐
        /// </summary>
        IS_LESS_THEN = 1 << 11,

        /// <summary>
        /// 초과
        /// OR_EQUAL와 함께 쓰일 경우 크거나 같다 의 의미를 가짐
        /// </summary>
        IS_GRETER_THEN = 1 << 12,
        #endregion

        #region order
        /// <summary>
        /// 내림차순으로 정렬 
        /// </summary>
        ASCENDING = 1 << 13,

        /// <summary>
        /// 오름차순으로 정렬
        /// </summary>
        DESCENDING = 1 << 14,
        #endregion

        /// <summary>
        /// 가져올 레코드의 개수를 설정합니다.
        /// </summary>
        LIMIT = 1 << 15,

        /// <summary>
        /// 쿼리할 칼럼을 설정
        /// </summary>
        WHERE = 1 << 16,
    }
}