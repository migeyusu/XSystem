namespace XSystem.Core.Domain
{
    public enum RecommendLevel : int
    {
        Recommend = 5,
        Frequently = 4,
        Valuable = 3,
        Sometimes = 2,
        /// <summary>
        /// 罕见
        /// </summary>
        Rare = 1,
        /// <summary>
        /// 排除
        /// </summary>
        Exclude = 0
    }
}