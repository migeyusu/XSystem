using System.Collections.Generic;

namespace Reptile
{
    /// <summary>
    /// ���湤����������ԭ��SQL��
    /// </summary>
    public class ReptileWorkFlow
    {
        //public string DataBase { get; set; }
        /// <summary>
        /// ȡ�ô�URL���е�datatable
        /// </summary>
        public string UrlSql { get; set; }
        /// <summary>
        /// URL��
        /// </summary>
        public string UrlColumn { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string ValTable { get; set; }
        /// <summary>
        /// �Ƿ����
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// �����������
        /// </summary>
        public List<string> ValColumns { get; set; }
        /// <summary>
        /// ��ʼ��id
        /// </summary>
        public int Sn { get; set; }
        /// <summary>
        /// ִ�гɹ�URL�б�־
        /// </summary>
        public string Sign { get; set; }
    }
}