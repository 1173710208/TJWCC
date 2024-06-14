namespace TJWCC.Code
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public class Md5
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <param name="code">加密位数16/32</param>
        /// <returns></returns>
        public static string md5(string str, int code)
        {
            string strEncrypt = string.Empty;
            if (code == 16)
            {
#pragma warning disable CS0618 // “FormsAuthentication.HashPasswordForStoringInConfigFile(string, string)”已过时:“The recommended alternative is to use the Membership APIs, such as Membership.CreateUser. For more information, see http://go.microsoft.com/fwlink/?LinkId=252463.”
                strEncrypt = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").Substring(8, 16);
#pragma warning restore CS0618 // “FormsAuthentication.HashPasswordForStoringInConfigFile(string, string)”已过时:“The recommended alternative is to use the Membership APIs, such as Membership.CreateUser. For more information, see http://go.microsoft.com/fwlink/?LinkId=252463.”
            }

            if (code == 32)
            {
#pragma warning disable CS0618 // “FormsAuthentication.HashPasswordForStoringInConfigFile(string, string)”已过时:“The recommended alternative is to use the Membership APIs, such as Membership.CreateUser. For more information, see http://go.microsoft.com/fwlink/?LinkId=252463.”
                strEncrypt = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
#pragma warning restore CS0618 // “FormsAuthentication.HashPasswordForStoringInConfigFile(string, string)”已过时:“The recommended alternative is to use the Membership APIs, such as Membership.CreateUser. For more information, see http://go.microsoft.com/fwlink/?LinkId=252463.”
            }

            return strEncrypt;
        }
    }
}
