using System.Configuration;

namespace Gazette.Infrastructure
{
    public class GazetteConfiguration : ConfigurationSection
    {
        private static readonly GazetteConfiguration Configuration = ConfigurationManager.GetSection("Gazette") as GazetteConfiguration;
        
        public static GazetteConfiguration Instance
        {
            get { return Configuration; }
        }

        [ConfigurationProperty("disqusShortName", DefaultValue = "", IsRequired = true)]
        public string DisqusShortName
        {
            get { return (string) this["disqusShortName"]; }
            set { this["disqusShortName"] = value; }
        }

        [ConfigurationProperty("blogName", DefaultValue = "", IsRequired = true)]
        public string BlogName
        {
            get { return (string) this["blogName"]; }
            set { this["blogName"] = value; }
        }

        [ConfigurationProperty("constantSalt", DefaultValue = "", IsRequired = false)]
        public string ConstantSalt
        {
            get { return (string) this["constantSalt"]; }
            set { this["constantSalt"] = value; }
        }
    }
}