using System.Configuration;

namespace Ligi.Web.Admin
{
    public class Config
    {
        public static string ConnectionStringName
        {
            get
            {
                if (ConfigurationManager.AppSettings["ConnectionStringName"] 
                    != null)
                {
                    return ConfigurationManager
                        .AppSettings["ConnectionStringName"];
                }

                return "DefaultConnection";
            }
        }

        public static string UsersTableName
        {
            get
            {
                if (ConfigurationManager.AppSettings["UsersTableName"] 
                    != null)
                {
                    return ConfigurationManager
                        .AppSettings["UsersTableName"];
                }

                return "Users";
            }
        }

        public static string UsersPrimaryKeyColumnName
        {
            get
            {
                if (ConfigurationManager.AppSettings["UsersPrimaryKeyColumnName"] 
                    != null)
                {
                    return ConfigurationManager
                        .AppSettings["UsersPrimaryKeyColumnName"];
                }

                return "Id";
            }
        }

        public static string UsersUserNameColumnName
        {
            get
            {
                if (ConfigurationManager.AppSettings["UsersUserNameColumnName"] 
                    != null)
                {
                    return ConfigurationManager
                        .AppSettings["UsersUserNameColumnName"];
                }

                return "Username";
            }
        }

        public static string ImagesFolderPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["ImagesFolderPath"] 
                    != null)
                {
                    return ConfigurationManager
                        .AppSettings["ImagesFolderPath"];
                }

                return "~/img/homes";
            }
        }

        public static string ImagesUrlPrefix
        {
            get  {return ImagesFolderPath.Replace("~", ""); }
        }

    }
}