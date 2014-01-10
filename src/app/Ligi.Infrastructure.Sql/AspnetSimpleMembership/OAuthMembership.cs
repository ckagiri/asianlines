using System.ComponentModel.DataAnnotations;

namespace Ligi.Infrastructure.Sql.AspnetSimpleMembership
{
	[ScaffoldTable(false)]
	public class OAuthMembershipMetadata { }

	[MetadataType(typeof(OAuthMembershipMetadata))]
	public class OAuthMembership
	{
		public string Provider {get;set;}
		public string ProviderUserId {get;set;}
		public int UserId { get; set; }
	}
}
