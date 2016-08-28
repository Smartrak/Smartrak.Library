namespace Permissions
{
	public interface IPermissionable<TUser> where TUser : class
	{
		bool HasPermissions(TUser user, string subscope = null);
	}
}
