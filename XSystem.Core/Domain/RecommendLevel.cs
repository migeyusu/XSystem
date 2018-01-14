namespace XSystem.Core.Domain
{
    public enum RecommendLevel : byte
    {
        Recommend = 1,
        Frequently = 2,
        Valuable = 3,
        Sometimes = 4,
        /// <summary>
        /// 罕见
        /// </summary>
        Rare = 5,
        /// <summary>
        /// 不建议显示
        /// </summary>
        Exclude = 6
    }
}